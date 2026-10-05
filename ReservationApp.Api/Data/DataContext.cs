using ReservationApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ReservationApp.Api.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationJournal> ReservationJournals { get; set; }

}
