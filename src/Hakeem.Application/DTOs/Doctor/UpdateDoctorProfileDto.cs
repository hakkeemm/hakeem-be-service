using Microsoft.AspNetCore.Http;

namespace Hakeem.Application.DTOs.Doctor;

public class UpdateDoctorProfileDto
{
    public List<Guid>? CategoryIds { get; set; }
    public string? AboutDescription { get; set; }
    public List<IFormFile>? Attachments { get; set; }
}
