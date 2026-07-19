using Hakeem.Application.DTOs.Common;
using Hakeem.Domain.Enums;

namespace Hakeem.Application.DTOs.Appointment;

public class AppointmentFilterDto : PaginationRequestDto
{
    public AppointmentStatus? Status { get; set; }
    public DateTime? Date { get; set; }
}
