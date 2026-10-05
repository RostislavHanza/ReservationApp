using ReservationApp.Api.Data;
using ReservationApp.Api.Models;
using System.Text.Json;

namespace ReservationApp.Api.Services;

public class JournalService : IJournalService
{
    private DataContext _db;
    public JournalService(DataContext dataContext)
    {
        _db = dataContext;
    }
    public async Task LogJournalEntry
        (Reservation oldR, Reservation newR, string ipAdress, string navigator, OperationType type)
    {
        ReservationJournal journal = new ReservationJournal
        {
            CreatedAt = DateTime.UtcNow,
            ReservationId = oldR.Id,
            ReservationCode = oldR.ReservationCode,
            IpAddress = ipAdress,
            Navigator = navigator
        };
        switch (type)
        {
            case OperationType.Created:
                journal.Operation = "Created";
                journal.NewData = JsonSerializer.Serialize(newR);
                break;

            case OperationType.Deleted:
                journal.Operation = "Deleted";
                journal.OldData = JsonSerializer.Serialize(oldR);
                break;

            case OperationType.Changed:
                var operations = new List<string>();

                if (oldR.TicketCount != newR.TicketCount)
                    operations.Add("Tickets Changed");

                if (oldR.Email != newR.Email)
                    operations.Add("Email Changed");

                if (oldR.Phone != newR.Phone)
                    operations.Add("Phone Changed");

                journal.Operation = string.Join("\n", operations);

                journal.OldData = JsonSerializer.Serialize(oldR);
                journal.NewData = JsonSerializer.Serialize(newR);
                break;
            default:
                break;
        }

        await _db.ReservationJournals.AddAsync(journal);
        return;

    }
}