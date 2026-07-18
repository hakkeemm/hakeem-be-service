using Hakeem.Application.DTOs.Doctor;
using Hakeem.Application.Interfaces;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Hakeem.Application.Services;

public class DoctorProfileService : IDoctorProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;

    public DoctorProfileService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
    {
        _unitOfWork = unitOfWork;
        _env = env;
    }

    public async Task<bool> UpdateProfileAsync(string doctorId, UpdateDoctorProfileDto dto)
    {
        var users = await _unitOfWork.Repository<ApplicationUser>().FindAsync(u => u.Id == doctorId);
        var user = users.FirstOrDefault();
        if (user == null) throw new Exception("Doctor not found");

        if (dto.AboutDescription != null)
        {
            user.AboutDescription = dto.AboutDescription;
        }

        // Handle Categories logic
        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            user.Categories.Clear(); // In a real EF context with generic repo, this might need explicit handling, but we assume tracking is on.
            foreach (var catId in dto.CategoryIds)
            {
                var cat = await _unitOfWork.Repository<Category>().GetByIdAsync(catId);
                if (cat != null)
                {
                    user.Categories.Add(cat);
                }
            }
        }

        _unitOfWork.Repository<ApplicationUser>().Update(user);

        // Handle File Uploads
        if (dto.Attachments != null && dto.Attachments.Any())
        {
            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            foreach (var file in dto.Attachments)
            {
                var folderName = GetFolderNameForFile(file.FileName);
                var uploadsFolder = Path.Combine(wwwroot, folderName);
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var attachment = new DoctorAttachment
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    Title = file.FileName,
                    FileUrl = $"/{folderName}/{uniqueFileName}",
                    FileType = Path.GetExtension(file.FileName)
                };

                await _unitOfWork.Repository<DoctorAttachment>().AddAsync(attachment);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private string GetFolderNameForFile(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLower();
        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp") return "images";
        if (ext == ".pdf") return "pdfs";
        if (ext == ".doc" || ext == ".docx") return "words";
        return "others";
    }
}
