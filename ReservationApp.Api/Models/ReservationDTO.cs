using System.ComponentModel.DataAnnotations;
namespace ReservationApp.Api.Models;

public class ReservationDTO
{
    [Required]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone must contain exactly 9 digits.")]
    public string Phone { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Ticket count must be between 1 and 10.")]
    public int TicketCount { get; set; }

    public string? ReservationCode { get; set; }
}
