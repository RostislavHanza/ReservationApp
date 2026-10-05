namespace ReservationApp.Api.Models;

public class Reservation
{
    public int Id { get; set; }

    public string ReservationCode { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int TicketCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
