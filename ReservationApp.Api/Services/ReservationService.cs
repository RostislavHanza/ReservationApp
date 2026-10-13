using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReservationApp.Api.Data;
using ReservationApp.Api.Models;

namespace ReservationApp.Api.Services;

public class ReservationService : IReservationService
{
    private readonly IJournalService _jr;
    private readonly DataContext _db;
    private readonly IConfiguration _config;

    public ReservationService(IJournalService journal, DataContext data, IConfiguration config)
    {
        _db = data;
        _jr = journal;
        _config = config;
    }
    public async Task ChangeReservation(ReservationDTO request, string ipAddress, string navigator)
    {
        await ExecuteInTransaction(async () =>
        {
            var oldReservation = await _db.Reservations
            .FirstOrDefaultAsync(r => r.ReservationCode == request.ReservationCode);

            if (oldReservation == null)
                throw new Exception("Reservation not found.");

            var ticketsCount = await _db.Reservations
                .Where(r => r.Id != oldReservation.Id)
                .SumAsync(r => r.TicketCount);
            int capacity = _config.GetValue<int>("Event:Capacity");

            if (ticketsCount + request.TicketCount > capacity)
                throw new Exception("Not enough tickets.");

            var newReservation = new Reservation
            {
                Id = oldReservation.Id,
                ReservationCode = oldReservation.ReservationCode,
                Email = request.Email,
                Phone = request.Phone,
                TicketCount = request.TicketCount,
                CreatedAt = oldReservation.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await _jr.LogJournalEntry
                (oldReservation, newReservation, ipAddress, navigator, OperationType.Changed);

            oldReservation.Email = newReservation.Email;
            oldReservation.Phone = newReservation.Phone;
            oldReservation.TicketCount = newReservation.TicketCount;
            oldReservation.UpdatedAt = newReservation.UpdatedAt;

            await _db.SaveChangesAsync();

            return true;
        });
    }

    public async Task<string> CreateReservation(ReservationDTO request, string ipAddress, string navigator)
    {
        return await ExecuteInTransaction(async () =>
        {
            var ticketsCount = await _db.Reservations
                    .SumAsync(r => r.TicketCount);

            int capacity = _config.GetValue<int>("Event:Capacity");

            if (ticketsCount + request.TicketCount > capacity)
                throw new Exception("Not enough tickets.");

            var newReservation = new Reservation
            {
                Email = request.Email,
                Phone = request.Phone,
                TicketCount = request.TicketCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _db.Reservations.AddAsync(newReservation);
            await _db.SaveChangesAsync();

            newReservation.ReservationCode = GenerateReservationCode(newReservation.Id);

            await _jr.LogJournalEntry
                (newReservation, newReservation, ipAddress, navigator, OperationType.Created);

            await _db.SaveChangesAsync();
            return newReservation.ReservationCode;
        });

    }

    public async Task DeleteReservation(string reservationCode, string ipAddress, string navigator)
    {
        await ExecuteInTransaction(async () =>
        {
            if (string.IsNullOrWhiteSpace(reservationCode))
                throw new Exception("Reservation code is required.");

            var reservation = await _db.Reservations
                .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode);

            if (reservation == null)
                throw new Exception("Reservation not found.");

            await _jr.LogJournalEntry(reservation, reservation, ipAddress, navigator, OperationType.Deleted);

            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync();
            return true;
        });
    }

    public async Task<ReservationDTO> GetReservation(string reservationCode)
    {
        if (string.IsNullOrWhiteSpace(reservationCode))
            throw new Exception("Reservation code is required.");

        var reservation = await _db.Reservations
            .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode);

        if (reservation == null)
            throw new Exception("Reservation not found.");

        return new ReservationDTO
        {
            Email = reservation.Email,
            Phone = reservation.Phone,
            TicketCount = reservation.TicketCount,
            ReservationCode = reservation.ReservationCode
        };

    }
    private async Task<T> ExecuteInTransaction<T>(Func<Task<T>> action)
    {
        var connection = (SqliteConnection)_db.Database.GetDbConnection();

        await _db.Database.OpenConnectionAsync();

        try
        {
            using var transaction = connection.BeginTransaction(deferred: false);

            await using var efTransaction = await _db.Database.UseTransactionAsync(transaction);

            try
            {
                var result = await action();

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
    private string GenerateReservationCode(int id)
    {
        var random = Guid.NewGuid().ToString("N").ToUpper();

        var prefix = random[..4];
        var suffix = random[4..8];

        return $"{prefix}-{id}-{suffix}";
    }
}
