using ReservationApp.Api.Models;

namespace ReservationApp.Api.Services;

public interface IJournalService
{
    Task LogJournalEntry
        (Reservation oldR, Reservation newR, string ipAdress, string navigator, OperationType type);
}
