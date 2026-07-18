using System.Security.Claims;
using Hakeem.Application.DTOs.Appointment;
using Hakeem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hakeem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // You can add Roles = "Patient" here when roles are fully set up
public class PatientAppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public PatientAppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return Unauthorized();

            var result = await _appointmentService.BookAppointmentAsync(patientId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-appointments")]
    public async Task<IActionResult> GetMyAppointments()
    {
        try
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return Unauthorized();

            var result = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        try
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return Unauthorized();

            var result = await _appointmentService.GetAppointmentByIdAsync(id, patientId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        try
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return Unauthorized();

            await _appointmentService.CancelAppointmentAsync(id, patientId);
            return Ok(new { message = "Appointment cancelled successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(Guid id, [FromBody] RescheduleAppointmentDto dto)
    {
        try
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return Unauthorized();

            var result = await _appointmentService.RescheduleAppointmentAsync(id, patientId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
