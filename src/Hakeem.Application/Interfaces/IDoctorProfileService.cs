using Hakeem.Application.DTOs.Doctor;

namespace Hakeem.Application.Interfaces;

public interface IDoctorProfileService
{
    Task<bool> UpdateProfileAsync(string doctorId, UpdateDoctorProfileDto dto);
}
