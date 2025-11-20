using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ASI.Basecode.WebApp.Models;

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
                if (string.IsNullOrWhiteSpace(username))
                {
                    return Ok(new List<BookingViewModel>());
                }
                
                var bookings = _bookingService.GetUserBookings(username);
                return Ok(bookings ?? new List<BookingViewModel>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserBookings: {ex.Message}");
                // Return empty list instead of error to prevent crashes
                return Ok(new List<BookingViewModel>());
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

                if (participants == null)
                {
                    return Ok(new List<object>());
                }

                return Ok(participants);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableParticipants: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return empty list instead of fallback to avoid confusion
                return Ok(new List<object>());
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
                    var apiNull = ApiResult<object>.CreateError("Booking data is required");
                    return BadRequest(apiNull);
                }

                // ModelState validation (Suppressed automatic filter in Startup to allow custom shape)
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? "Invalid value" : e.ErrorMessage).ToArray()
                    );

                    var apiErr = ApiResult<object>.CreateError("Validation failed");
                    apiErr.Response = errors;
                    return BadRequest(apiErr);
                }

                // Check availability before saving
                var isAvailable = _bookingService.CheckRoomAvailability(booking.RoomId, booking.Date, booking.StartTime, booking.EndTime, null);
                if (!isAvailable)
                {
                    var apiErr = ApiResult<object>.CreateError("Room not available for selected time");
                    apiErr.Response = new Dictionary<string, string[]>
                    {
                        { "StartTime", new[] { "Room is not available for the selected time slot." } }
                    };
                    return BadRequest(apiErr);
                }

                _bookingService.AddBooking(booking);
                return Ok(ApiResult<object>.CreateSuccess(null, "Booking added successfully"));
            }
            catch (Exception ex)
            {
                var apiEx = ApiResult<object>.CreateError(ex.Message);
                apiEx.Response = new { innerException = ex.InnerException?.Message };
                return BadRequest(apiEx);
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
                if (roomId <= 0)
                {
                    return Ok(new { available = false, error = "Invalid roomId" });
                }

                if (string.IsNullOrWhiteSpace(startTime) || string.IsNullOrWhiteSpace(endTime))
                {
                    return Ok(new { available = false, error = "StartTime and EndTime are required" });
                }

                var isAvailable = _bookingService.CheckRoomAvailability(roomId, date, startTime, endTime, excludeBookingId);
                return Ok(new { available = isAvailable });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckAvailability: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Ok(new { available = false, error = $"Failed to check room availability: {ex.Message}" });
            }
        }

        [HttpGet("Health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.Now });
        }

        [HttpPost("ValidateBooking")]
        [AllowAnonymous]
        public IActionResult ValidateBooking([FromBody] BookingViewModel booking)
        {
            try
            {
                if (booking == null)
                {
                    return Ok(ApiResult<object>.CreateError("Booking data is required"));
                }

                var validationErrors = new Dictionary<string, string[]>();
                var isValid = true;

                // ModelState validation
                if (!ModelState.IsValid)
                {
                    isValid = false;
                    foreach (var kvp in ModelState)
                    {
                        if (kvp.Value.Errors.Count > 0)
                        {
                            validationErrors[kvp.Key] = kvp.Value.Errors
                                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) 
                                    ? e.Exception?.Message ?? "Invalid value" 
                                    : e.ErrorMessage)
                                .ToArray();
                        }
                    }
                }

                // Custom validation using IValidatableObject
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(booking);
                if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(booking, validationContext, validationResults, true))
                {
                    isValid = false;
                    foreach (var result in validationResults)
                    {
                        foreach (var memberName in result.MemberNames)
                        {
                            if (!validationErrors.ContainsKey(memberName))
                            {
                                validationErrors[memberName] = new string[] { };
                            }
                            var existingErrors = validationErrors[memberName].ToList();
                            existingErrors.Add(result.ErrorMessage);
                            validationErrors[memberName] = existingErrors.ToArray();
                        }
                    }
                }

                // Check room availability if basic validation passes
                if (isValid && booking.RoomId > 0 && booking.Date != default && 
                    !string.IsNullOrWhiteSpace(booking.StartTime) && !string.IsNullOrWhiteSpace(booking.EndTime))
                {
                    var isAvailable = _bookingService.CheckRoomAvailability(booking.RoomId, booking.Date, booking.StartTime, booking.EndTime, booking.Id > 0 ? booking.Id : null);
                    if (!isAvailable)
                    {
                        isValid = false;
                        if (!validationErrors.ContainsKey("StartTime"))
                        {
                            validationErrors["StartTime"] = new string[] { };
                        }
                        var existingErrors = validationErrors["StartTime"].ToList();
                        existingErrors.Add("Room is not available for the selected time slot.");
                        validationErrors["StartTime"] = existingErrors.ToArray();
                    }
                }

                if (isValid)
                {
                    return Ok(ApiResult<object>.CreateSuccess("Booking data is valid"));
                }
                else
                {
                    var apiErr = ApiResult<object>.CreateError("Validation failed");
                    apiErr.Response = validationErrors;
                    return Ok(apiErr);
                }
            }
            catch (Exception ex)
            {
                var apiEx = ApiResult<object>.CreateError(ex.Message);
                return BadRequest(apiEx);
            }
        }

        [HttpPost("ValidateBookingFields")]
        [AllowAnonymous]
        public IActionResult ValidateBookingFields([FromBody] BookingViewModel booking)
        {
            try
            {
                if (booking == null)
                {
                    return Ok(ApiResult<object>.CreateError("Booking data is required"));
                }

                var fieldErrors = new Dictionary<string, string[]>();

                // Validate required fields
                if (booking.RoomId <= 0)
                {
                    fieldErrors["RoomId"] = new[] { "RoomId is required and must be greater than 0" };
                }

                if (string.IsNullOrWhiteSpace(booking.RoomName))
                {
                    fieldErrors["RoomName"] = new[] { "RoomName is required" };
                }

                if (string.IsNullOrWhiteSpace(booking.Floor))
                {
                    fieldErrors["Floor"] = new[] { "Floor is required" };
                }

                if (booking.Date == default)
                {
                    fieldErrors["Date"] = new[] { "Date is required and must be valid" };
                }
                else if (booking.Date < DateTime.Today)
                {
                    fieldErrors["Date"] = new[] { "Date cannot be in the past" };
                }

                if (string.IsNullOrWhiteSpace(booking.StartTime))
                {
                    fieldErrors["StartTime"] = new[] { "StartTime is required" };
                }

                if (string.IsNullOrWhiteSpace(booking.EndTime))
                {
                    fieldErrors["EndTime"] = new[] { "EndTime is required" };
                }

                // Validate time format and order
                if (!string.IsNullOrWhiteSpace(booking.StartTime) && !string.IsNullOrWhiteSpace(booking.EndTime))
                {
                    if (TimeSpan.TryParse(booking.StartTime, out var start) && TimeSpan.TryParse(booking.EndTime, out var end))
                    {
                        if (start >= end)
                        {
                            if (!fieldErrors.ContainsKey("StartTime"))
                            {
                                fieldErrors["StartTime"] = new string[] { };
                            }
                            var existingErrors = fieldErrors["StartTime"].ToList();
                            existingErrors.Add("StartTime must be before EndTime");
                            fieldErrors["StartTime"] = existingErrors.ToArray();
                        }
                    }
                    else
                    {
                        if (!fieldErrors.ContainsKey("StartTime"))
                        {
                            fieldErrors["StartTime"] = new string[] { };
                        }
                        var existingErrors = fieldErrors["StartTime"].ToList();
                        existingErrors.Add("StartTime or EndTime have invalid format. Expected format: HH:mm");
                        fieldErrors["StartTime"] = existingErrors.ToArray();
                    }
                }

                // Validate recurring booking fields if Recurring is true
                if (booking.Recurring)
                {
                    if (string.IsNullOrWhiteSpace(booking.Frequency))
                    {
                        fieldErrors["Frequency"] = new[] { "Frequency is required for recurring bookings" };
                    }

                    if (!booking.RecurringEndDate.HasValue)
                    {
                        fieldErrors["RecurringEndDate"] = new[] { "RecurringEndDate is required for recurring bookings" };
                    }
                    else if (booking.RecurringEndDate.Value < booking.Date)
                    {
                        fieldErrors["RecurringEndDate"] = new[] { "RecurringEndDate cannot be before the booking date" };
                    }

                    if (booking.DaysOfWeek == null || booking.DaysOfWeek.Count == 0)
                    {
                        fieldErrors["DaysOfWeek"] = new[] { "DaysOfWeek is required for recurring bookings" };
                    }
                }

                if (fieldErrors.Count == 0)
                {
                    return Ok(ApiResult<object>.CreateSuccess("All fields are valid"));
                }
                else
                {
                    var apiErr = ApiResult<object>.CreateError("Field validation failed");
                    apiErr.Response = fieldErrors;
                    return Ok(apiErr);
                }
            }
            catch (Exception ex)
            {
                var apiEx = ApiResult<object>.CreateError(ex.Message);
                return BadRequest(apiEx);
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