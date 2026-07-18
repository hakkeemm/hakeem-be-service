using System.Security.Claims;
using Hakeem.Application.DTOs.Doctor;
using Hakeem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hakeem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Doctor")]
public class DoctorProfileController : ControllerBase
{
    private readonly IDoctorProfileService _doctorProfileService;

    public DoctorProfileController(IDoctorProfileService doctorProfileService)
    {
        _doctorProfileService = doctorProfileService;
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateDoctorProfileDto dto)
    {
        try
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId)) return Unauthorized();

            var result = await _doctorProfileService.UpdateProfileAsync(doctorId, dto);
            return Ok(new { message = "Profile updated successfully.", success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
