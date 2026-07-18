using System.Security.Claims;
using Hakeem.Application.DTOs.Appointment;
using Hakeem.Application.DTOs.Schedule;
using Hakeem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hakeem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Doctor,Assistant")]
public class ClinicAppointmentsController : ControllerBase
{
    private readonly IDoctorScheduleService _scheduleService;
    private readonly IAppointmentService _appointmentService;

    public ClinicAppointmentsController(
        IDoctorScheduleService scheduleService, 
        IAppointmentService appointmentService)
    {
        _scheduleService = scheduleService;
        _appointmentService = appointmentService;
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> GetClinicSchedule()
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            // Note: Add logic in service to fetch based on staffId mapping to DoctorId
            return Ok(new { message = "Schedule endpoint needs specific fetch logic" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("schedule")]
    public async Task<IActionResult> AddSchedule([FromBody] CreateDoctorScheduleDto dto)
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _scheduleService.AddScheduleAsync(staffId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("book-appointment")]
    public async Task<IActionResult> BookAppointmentForDoctor(string patientId, [FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var result = await _appointmentService.BookAppointmentAsync(patientId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/check-in")]
    public async Task<IActionResult> CheckInPatient(Guid id)
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _appointmentService.CheckInPatientAsync(id, staffId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(Guid id)
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _appointmentService.ConfirmAppointmentAsync(id, staffId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteAppointment(Guid id)
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _appointmentService.CompleteAppointmentAsync(id, staffId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
