using ReservationApp.Api.Models;

namespace ReservationApp.Api.Services;

public interface IReservationService
{
    Task<string> CreateReservation(ReservationDTO request, string ipAddress, string navigator);
    Task ChangeReservation(ReservationDTO request, string ipAddress, string navigator);
    Task DeleteReservation(string reservationCode, string ipAddress, string navigator);
    Task<ReservationDTO> GetReservation(string reservationCode);
}
