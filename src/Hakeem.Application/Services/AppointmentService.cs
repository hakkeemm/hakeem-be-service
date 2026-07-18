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
