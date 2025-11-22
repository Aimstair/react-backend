using ASI.Basecode.Services.DTOs;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("UserBookings/{username}")]
        [AllowAnonymous]
        public IActionResult GetUserBookings(string username)
        {
            var bookings = _bookingService.GetUserBookings(username);
            return Ok(bookings);
        }

        [HttpGet("UpcomingUserBookings/{username}")]
        [AllowAnonymous]
        public IActionResult GetUpcomingUserBookings(string username)
        {
            var bookings = _bookingService.GetUpcomingUserBookings(username);
            return Ok(bookings);
        }

        [HttpGet("PastUserBookings/{username}")]
        [AllowAnonymous]
        public IActionResult GetPastUserBookings(string username)
        {
            var bookings = _bookingService.GetPastUserBookings(username);
            return Ok(bookings);
        }

        [HttpPost("AddBooking")]
        [AllowAnonymous]
        public async Task<IActionResult> AddBooking([FromBody] AddBookingDto dto)
        {
            if (dto == null) 
                return BadRequest(new { message = "Booking data is required" });

            var bookingVm = new BookingViewModel
            {
                RoomId = dto.RoomId,
                RoomName = dto.RoomName,
                Floor = dto.Floor,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Purpose = dto.Purpose,
                Organizer = dto.Organizer,
                Recurring = dto.Recurring,
                Frequency = dto.Frequency,
                RecurringEndDate = dto.RecurringEndDate,
                DaysOfWeek = dto.DaysOfWeek ?? new List<string>(),
                Image = dto.Image,
                Participants = dto.Participants ?? new List<string>(),
                Amenities = dto.Amenities ?? new List<string>()
            };

            if (dto.Recurring)
            {
                var recurrenceDates = CalculateRecurrenceDates(dto);

                var suggestions = await _bookingService.GetRecurringRoomSuggestionsAsync(bookingVm, recurrenceDates);

                return Ok(new { message = "Recurring booking suggestions generated", suggestions });
            }
            else
            {
                _bookingService.AddBooking(bookingVm);
                return Ok(new { message = "Booking added successfully" });
            }
        }

        private List<DateTime> CalculateRecurrenceDates(AddBookingDto dto)
        {
            var dates = new List<DateTime>();
            if (!dto.Recurring || dto.RecurringEndDate == null) return dates;

            var currentDate = dto.Date;
            while (currentDate <= dto.RecurringEndDate.Value)
            {
                if (dto.DaysOfWeek == null || !dto.DaysOfWeek.Any() ||
                    dto.DaysOfWeek.Any(d => string.Equals(d, currentDate.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    dates.Add(currentDate);
                }

                currentDate = dto.Frequency?.ToLower() switch
                {
                    "daily" => currentDate.AddDays(1),
                    "weekly" => currentDate.AddDays(7),
                    "monthly" => currentDate.AddMonths(1),
                    _ => currentDate.AddDays(1)
                };
            }

            return dates;
        }

        [HttpPut("UpdateBooking/{id}")]
        [AllowAnonymous]
        public IActionResult UpdateBooking(int id, [FromBody] UpdateBookingDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Booking data is required" });

            var bookingVm = new BookingViewModel
            {
                Id = id,
                RoomId = dto.RoomId,
                RoomName = dto.RoomName,
                Floor = dto.Floor,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Purpose = dto.Purpose,
                Organizer = dto.Organizer,
                Recurring = dto.Recurring,
                Frequency = dto.Frequency,
                RecurringEndDate = dto.RecurringEndDate,
                DaysOfWeek = dto.DaysOfWeek ?? new List<string>(),
                Image = dto.Image,
                Participants = dto.Participants ?? new List<string>(),
                Amenities = dto.Amenities ?? new List<string>()
            };

            _bookingService.UpdateBooking(bookingVm);
            return Ok(new { message = "Booking updated successfully" });
        }

        [HttpDelete("DeleteBooking/{id}")]
        [AllowAnonymous]
        public IActionResult DeleteBooking(int id)
        {
            _bookingService.DeleteBooking(id);
            return Ok(new { message = "Booking deleted successfully" });
        }

        [HttpGet("CheckAvailability")]
        [AllowAnonymous]
        public IActionResult CheckAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null)
        {
            var isAvailable = _bookingService.CheckRoomAvailability(roomId, date, startTime, endTime, excludeBookingId);
            return Ok(new { available = isAvailable });
        }
    }
}
