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

    [HttpGet]
    public async Task<IActionResult> GetAllAppointments([FromQuery] Hakeem.Application.DTOs.Appointment.AppointmentFilterDto filter)
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _appointmentService.GetClinicAppointmentsAsync(staffId, filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> GetClinicSchedule()
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _scheduleService.GetClinicScheduleAsync(staffId);
            return Ok(result);
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

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        try
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, staffId, dto.Status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
