using System.Security.Claims;
using Hakeem.Application.DTOs.Appointment;
using Hakeem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hakeem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized();

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
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized();

            var result = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
