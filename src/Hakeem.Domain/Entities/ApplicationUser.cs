using Microsoft.AspNetCore.Identity;
using Hakeem.Domain.Enums;

using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class ApplicationUser : IdentityUser, IAuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiresAt { get; set; }
    
    // Doctor Specific Fields
    public Guid? SpecialtyId { get; set; }
    public Category? Specialty { get; set; }
    
    public int? YearsExperience { get; set; }
    public decimal? NewVisitFee { get; set; }
    public decimal? FollowUpFee { get; set; }
    public decimal? ConsultationFee { get; set; }
    public decimal? QuickVisitFee { get; set; }
    public int? AdvanceBookingDays { get; set; }
    public decimal? AverageRating { get; set; }
    public int? ReviewsCount { get; set; }
    public int? PatientsCount { get; set; }
    public string? AboutDescription { get; set; }
    public bool? IsPopular { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Patient Specific Fields
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    
    // Assistant Specific Fields
    public string? AssignedDoctorId { get; set; }
    public ApplicationUser? AssignedDoctor { get; set; }
    
    // Navigation Properties
    
    // As a Doctor
    public ICollection<ApplicationUser> Assistants { get; set; } = new List<ApplicationUser>();
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    public ICollection<DoctorScheduleException> ScheduleExceptions { get; set; } = new List<DoctorScheduleException>();

    public ICollection<Appointment> ReceivedAppointments { get; set; } = new List<Appointment>();
    public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
    public ICollection<Favorite> FavoritedBy { get; set; } = new List<Favorite>();
    public ICollection<DoctorAttachment> Attachments { get; set; } = new List<DoctorAttachment>();
    
    // As a Patient
    public ICollection<Appointment> BookedAppointments { get; set; } = new List<Appointment>();
    public ICollection<Review> WrittenReviews { get; set; } = new List<Review>();
    public ICollection<Favorite> SavedFavorites { get; set; } = new List<Favorite>();
}
