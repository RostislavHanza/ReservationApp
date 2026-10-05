namespace ReservationApp.Api.Models;

public class ReservationJournal
{
    public int Id { get; set; }

    public int ReservationId { get; set; }

    public string ReservationCode { get; set; } = string.Empty;

    public string Operation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? IpAddress { get; set; }

    public string? Navigator { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }
}
public enum OperationType
{
    Created,
    Deleted,
    Changed
}

