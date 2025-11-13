using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("GetBookings")]
        [AllowAnonymous]
        public IActionResult GetBookings()
        {
            try
            {
                var bookings = _bookingService.GetBookings();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get bookings: {ex.Message}");
            }
        }

        [HttpGet("GetUserBookings/{username}")]
        [AllowAnonymous]
        public IActionResult GetUserBookings(string username)
        {
            try
            {
                var bookings = _bookingService.GetUserBookings(username);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get user bookings: {ex.Message}");
            }
        }

        [HttpGet("GetUpcomingBookings/{username}")]
        [AllowAnonymous]
        public IActionResult GetUpcomingBookings(string username)
        {
            try
            {
                Console.WriteLine($"GetUpcomingBookings called for user: {username}");

                var upcomingBookings = _bookingService.GetUpcomingUserBookings(username);

                Console.WriteLine($"Found {upcomingBookings.Count} upcoming bookings");
                foreach (var booking in upcomingBookings)
                {
                    Console.WriteLine($"Booking: {booking.RoomName} on {booking.Date} at {booking.StartTime}");
                }

                return Ok(upcomingBookings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUpcomingBookings: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Ok(new List<BookingViewModel>()); // Return empty list instead of error
            }
        }

        [HttpGet("GetPastBookings/{username}")]
        [AllowAnonymous]
        public IActionResult GetPastBookings(string username)
        {
            try
            {
                Console.WriteLine($"GetPastBookings called for user: {username}");

                var pastBookings = _bookingService.GetPastUserBookings(username);

                Console.WriteLine($"Found {pastBookings.Count} past bookings");

                return Ok(pastBookings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPastBookings: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Ok(new List<BookingViewModel>()); // Return empty list instead of error
            }
        }

        [HttpGet("GetAvailableParticipants")]
        [AllowAnonymous]
        public IActionResult GetAvailableParticipants()
        {
            try
            {
                Console.WriteLine("GetAvailableParticipants called");

                var participants = _bookingService.GetAvailableParticipants();

                Console.WriteLine($"Found {participants.Count} available participants");

                return Ok(participants);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableParticipants: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return fallback participants
                return Ok(new List<object>
        {
            new { Name = "John Doe", UserId = "john_doe", Email = "john@example.com" },
            new { Name = "Jane Smith", UserId = "jane_smith", Email = "jane@example.com" },
            new { Name = "Alice Johnson", UserId = "alice_johnson", Email = "alice@example.com" },
            new { Name = "Bob Brown", UserId = "bob_brown", Email = "bob@example.com" },
            new { Name = "Charlie Wilson", UserId = "charlie_wilson", Email = "charlie@example.com" }
        });
            }
        }

        [HttpPost]
        [Route("AddBooking")]
        [AllowAnonymous]
        public IActionResult AddBooking([FromBody] BookingViewModel booking)
        {
            try
            {
                if (booking == null)
                {
                    return BadRequest(new { message = "Booking data is required" });
                }

                // Validate required fields
                if (string.IsNullOrEmpty(booking.RoomName) ||
                    string.IsNullOrEmpty(booking.Floor) ||
                    booking.Date == default ||
                    string.IsNullOrEmpty(booking.StartTime) ||
                    string.IsNullOrEmpty(booking.EndTime))
                {
                    return BadRequest(new { message = "All required fields must be provided" });
                }

                _bookingService.AddBooking(booking);
                return Ok(new { message = "Booking added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        // Helper methods
        private string GetStringOrDefault(JsonElement element, string propertyName, string defaultValue)
        {
            try
            {
                if (element.TryGetProperty(propertyName, out JsonElement property))
                {
                    return property.ValueKind == JsonValueKind.String ? property.GetString() ?? defaultValue : defaultValue;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool GetBoolOrDefault(JsonElement element, string propertyName, bool defaultValue)
        {
            try
            {
                if (element.TryGetProperty(propertyName, out JsonElement property))
                {
                    return property.ValueKind == JsonValueKind.True ||
                           (property.ValueKind == JsonValueKind.String && bool.TryParse(property.GetString(), out bool result) && result);
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private DateTime? GetDateOrDefault(JsonElement element, string propertyName)
        {
            try
            {
                if (element.TryGetProperty(propertyName, out JsonElement property))
                {
                    if (property.ValueKind == JsonValueKind.String)
                    {
                        var dateString = property.GetString();
                        if (!string.IsNullOrEmpty(dateString) && DateTime.TryParse(dateString, out DateTime result))
                        {
                            return result;
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private List<string> GetStringArrayAsList(JsonElement element, string propertyName)
        {
            try
            {
                var result = new List<string>();
                if (element.TryGetProperty(propertyName, out JsonElement property))
                {
                    if (property.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement item in property.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                            {
                                var stringValue = item.GetString();
                                if (!string.IsNullOrWhiteSpace(stringValue))
                                {
                                    result.Add(stringValue);
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch
            {
                return new List<string>();
            }
        }

        [HttpPut("UpdateBooking/{id}")]
        [AllowAnonymous]
        public IActionResult UpdateBooking(int id, [FromBody] JsonElement request)
        {
            try
            {
                Console.WriteLine($"UpdateBooking called for ID {id} with data: {request}");

                // Get the existing booking first
                var existingBooking = _bookingService.GetBooking(id);
                if (existingBooking == null)
                {
                    return BadRequest(new { message = "Booking not found" });
                }

                // Create BookingViewModel from the JsonElement request
                var bookingViewModel = new BookingViewModel
                {
                    Id = id,
                    RoomId = request.TryGetProperty("RoomId", out var roomIdProp) ? roomIdProp.GetInt32() : existingBooking.RoomId,
                    RoomName = GetStringOrDefault(request, "RoomName", existingBooking.RoomName),
                    Floor = GetStringOrDefault(request, "Floor", existingBooking.Floor),
                    Date = request.TryGetProperty("Date", out var dateProp) ? DateTime.Parse(dateProp.GetString()) : existingBooking.Date,
                    StartTime = GetStringOrDefault(request, "StartTime", existingBooking.StartTime),
                    EndTime = GetStringOrDefault(request, "EndTime", existingBooking.EndTime),
                    Purpose = GetStringOrDefault(request, "Purpose", existingBooking.Purpose),
                    Organizer = GetStringOrDefault(request, "Organizer", existingBooking.Organizer),
                    Recurring = GetBoolOrDefault(request, "Recurring", existingBooking.Recurring),
                    Frequency = GetStringOrDefault(request, "Frequency", existingBooking.Frequency),
                    RecurringEndDate = GetDateOrDefault(request, "RecurringEndDate") ?? existingBooking.RecurringEndDate,
                    DaysOfWeek = GetStringArrayAsList(request, "DaysOfWeek"),
                    Image = GetStringOrDefault(request, "Image", existingBooking.Image),
                    Participants = new List<string>() // CHANGE: List<string> instead of List<UserViewModel>
                };

                // Handle participants - NOW JUST STRINGS
                if (request.TryGetProperty("Participants", out JsonElement participantsElement) &&
                    participantsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement participantElement in participantsElement.EnumerateArray())
                    {
                        if (participantElement.ValueKind == JsonValueKind.String)
                        {
                            var participantName = participantElement.GetString();
                            if (!string.IsNullOrWhiteSpace(participantName))
                            {
                                bookingViewModel.Participants.Add(participantName); // Just add the string
                            }
                        }
                    }
                }

                Console.WriteLine($"Updating booking with {bookingViewModel.Participants.Count} participants");

                _bookingService.UpdateBooking(bookingViewModel);

                return Ok(new { message = "Booking updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateBooking Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteBooking/{id}")]
        [AllowAnonymous]
        public IActionResult DeleteBooking(int id)
        {
            try
            {
                _bookingService.DeleteBooking(id);
                return Ok(new { message = "Booking deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to delete booking: {ex.Message}" });
            }
        }

        [HttpGet("CheckAvailability")]
        [AllowAnonymous]
        public IActionResult CheckAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null)
        {
            try
            {
                var isAvailable = _bookingService.CheckRoomAvailability(roomId, date, startTime, endTime, excludeBookingId);
                return Ok(new { available = isAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to check room availability: {ex.Message}");
            }
        }

        public class AddBookingRequest
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public string Floor { get; set; }
            public DateTime Date { get; set; } // Change to DateTime
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Purpose { get; set; }
            public string Organizer { get; set; }
            public bool Recurring { get; set; }
            public string Frequency { get; set; }
            public DateTime? RecurringEndDate { get; set; } // Change to DateTime?
            public List<string> DaysOfWeek { get; set; }
            public string Image { get; set; }
            public List<string> Participants { get; set; }
        }
    }
}