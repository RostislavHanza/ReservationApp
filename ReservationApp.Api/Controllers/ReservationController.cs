using Microsoft.AspNetCore.Mvc;
using ReservationApp.Api.Models;
using ReservationApp.Api.Services;

namespace ReservationApp.Api.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _rs;
        public ReservationController(IReservationService reservationService)
        {
            _rs = reservationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationDTO reservation)
        {
            try
            {
                var code = await _rs.CreateReservation(reservation, GetIp(), GetNavigator());
                return Created("", code);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change")]
        public async Task<IActionResult> ChangeReservation([FromBody] ReservationDTO reservation)
        {
            try
            {
                await _rs.ChangeReservation(reservation, GetIp(), GetNavigator());
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get/{code}")]
        public async Task<IActionResult> GetReservation([FromRoute] string code)
        {
            try
            {
                var reservation = await _rs.GetReservation(code);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{code}")]
        public async Task<IActionResult> DeleteReservation([FromRoute] string code)
        {
            try
            {
                await _rs.DeleteReservation(code, GetIp(), GetNavigator());
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private string GetIp()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private string GetNavigator()
        {
            return Request.Headers.UserAgent.ToString();
        }
    }
}
