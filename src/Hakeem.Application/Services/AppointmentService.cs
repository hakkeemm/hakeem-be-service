using Hakeem.Application.DTOs.Appointment;
using Hakeem.Application.Interfaces;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Enums;
using Hakeem.Domain.Interfaces;

namespace Hakeem.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponseDto> BookAppointmentAsync(string patientId, CreateAppointmentDto dto)
    {
        var doctorRepo = _unitOfWork.Repository<ApplicationUser>();
        var doctors = await doctorRepo.FindAsync(d => d.Id == dto.DoctorId);
        var doctor = doctors.FirstOrDefault();
        
        if (doctor == null) throw new Exception("Doctor not found");

        decimal fee = dto.VisitPurpose switch
        {
            VisitPurpose.NewVisit => doctor.NewVisitFee ?? 0,
            VisitPurpose.FollowUp => doctor.FollowUpFee ?? 0,
            VisitPurpose.Consultation => doctor.ConsultationFee ?? 0,
            VisitPurpose.QuickVisit => doctor.QuickVisitFee ?? 0,
            _ => 0
        };

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = dto.DoctorId,
            AppointmentDate = dto.AppointmentDate,
            StartTime = dto.StartTime,
            EndTime = dto.StartTime.Add(TimeSpan.FromMinutes(30)),
            VisitType = dto.VisitType,
            VisitPurpose = dto.VisitPurpose,
            Status = AppointmentStatus.Booked,
            Fee = fee,
            Notes = dto.Notes
        };

        await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(appointment);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetPatientAppointmentsAsync(string patientId)
    {
        var appointments = await _unitOfWork.Repository<Appointment>()
            .FindAsync(a => a.PatientId == patientId);
            
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(Guid id, string patientId)
    {
        var appointments = await _unitOfWork.Repository<Appointment>().FindAsync(a => a.Id == id && a.PatientId == patientId);
        var appointment = appointments.FirstOrDefault();

        if (appointment == null) throw new Exception("Appointment not found or you do not have permission to view it.");

        return MapToDto(appointment);
    }

    public async Task<bool> CancelAppointmentAsync(Guid id, string patientId)
    {
        var appointments = await _unitOfWork.Repository<Appointment>().FindAsync(a => a.Id == id && a.PatientId == patientId);
        var appointment = appointments.FirstOrDefault();

        if (appointment == null) throw new Exception("Appointment not found or you do not have permission to cancel it.");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new Exception("Cannot cancel an already completed appointment.");

        appointment.Status = AppointmentStatus.Cancelled;
        
        _unitOfWork.Repository<Appointment>().Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<AppointmentResponseDto> RescheduleAppointmentAsync(Guid id, string patientId, RescheduleAppointmentDto dto)
    {
        var appointments = await _unitOfWork.Repository<Appointment>().FindAsync(a => a.Id == id && a.PatientId == patientId);
        var appointment = appointments.FirstOrDefault();

        if (appointment == null) throw new Exception("Appointment not found or you do not have permission to reschedule it.");

        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Cannot reschedule a completed or cancelled appointment.");

        appointment.AppointmentDate = dto.NewAppointmentDate;
        appointment.StartTime = dto.NewStartTime;
        appointment.EndTime = dto.NewStartTime.Add(TimeSpan.FromMinutes(30));
        appointment.Status = AppointmentStatus.Rescheduled;

        _unitOfWork.Repository<Appointment>().Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(appointment);
    }

    private async Task<string> GetDoctorIdForStaffAsync(string staffId)
    {
        var users = await _unitOfWork.Repository<ApplicationUser>().FindAsync(u => u.Id == staffId);
        var user = users.FirstOrDefault();
        if (user == null) throw new Exception("User not found.");

        if (user.Role == UserRole.Doctor) return user.Id;
        if (user.Role == UserRole.Assistant && !string.IsNullOrEmpty(user.AssignedDoctorId)) return user.AssignedDoctorId;

        throw new Exception("You do not have permission to manage clinic appointments.");
    }

    public async Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(Guid id, string staffId, AppointmentStatus newStatus)
    {
        var doctorId = await GetDoctorIdForStaffAsync(staffId);

        var appointments = await _unitOfWork.Repository<Appointment>().FindAsync(a => a.Id == id && a.DoctorId == doctorId);
        var appointment = appointments.FirstOrDefault();

        if (appointment == null) throw new Exception("Appointment not found or not assigned to your clinic.");

        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Cannot change the status of a completed or cancelled appointment.");

        // Optional: Ensure staff can only set specific valid statuses (e.g. they shouldn't set it to 'Booked' manually)
        if (newStatus == AppointmentStatus.Booked)
            throw new Exception("Cannot manually set status back to Booked.");

        appointment.Status = newStatus;
        _unitOfWork.Repository<Appointment>().Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(appointment);
    }

    private static AppointmentResponseDto MapToDto(Appointment appointment)
    {
        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            VisitType = appointment.VisitType,
            VisitPurpose = appointment.VisitPurpose,
            Status = appointment.Status,
            Fee = appointment.Fee,
            Notes = appointment.Notes
        };
    }
}
