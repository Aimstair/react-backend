using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookingService : ServiceBase, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RecurringSuggestionService _recurringSuggestionService;

        public BookingService(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory,
            RecurringSuggestionService recurringSuggestionService)
            : base(loggerFactory)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _recurringSuggestionService = recurringSuggestionService;
        }

        #region Booking CRUD

        public void AddBooking(BookingViewModel bookingViewModel)
        {
            try
            {
                var booking = MapToBooking(bookingViewModel);

                _bookingRepository.AddBooking(booking);
                _unitOfWork.SaveChanges();

                AddBookingParticipants(bookingViewModel.Participants, booking.Id);

                if (bookingViewModel.Amenities != null && bookingViewModel.Amenities.Any())
                    AddBookingAmenities(bookingViewModel.Amenities, booking.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding booking: {ex.Message}", ex.InnerException);
            }
        }

        public void UpdateBooking(BookingViewModel bookingViewModel)
        {
            try
            {
                var booking = _bookingRepository.GetBooking(bookingViewModel.Id);
                if (booking == null)
                    throw new Exception($"Booking with ID {bookingViewModel.Id} not found");

                booking.RoomId = bookingViewModel.RoomId;
                booking.RoomName = bookingViewModel.RoomName;
                booking.Floor = bookingViewModel.Floor;
                booking.Date = bookingViewModel.Date;
                booking.StartTime = bookingViewModel.StartTime;
                booking.EndTime = bookingViewModel.EndTime;
                booking.Purpose = bookingViewModel.Purpose;
                booking.Organizer = bookingViewModel.Organizer;
                booking.Recurring = bookingViewModel.Recurring;
                booking.Frequency = bookingViewModel.Frequency;
                booking.RecurringEndDate = bookingViewModel.RecurringEndDate;
                booking.DaysOfWeek = bookingViewModel.DaysOfWeek != null && bookingViewModel.DaysOfWeek.Any()
                    ? string.Join(",", bookingViewModel.DaysOfWeek)
                    : null;
                booking.UpdatedTime = DateTime.Now;
                booking.UpdatedBy = bookingViewModel.Organizer;

                _bookingRepository.UpdateBooking(booking);
                _unitOfWork.SaveChanges();

                if (bookingViewModel.Amenities != null && bookingViewModel.Amenities.Any())
                    UpdateBookingAmenities(bookingViewModel.Amenities, booking.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating booking: {ex.Message}", ex);
            }
        }

        public void DeleteBooking(int id)
        {
            try
            {
                var booking = _bookingRepository.GetBooking(id);
                if (booking == null)
                    throw new Exception($"Booking with ID {id} not found");

                _bookingRepository.DeleteBooking(booking);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting booking: {ex.Message}", ex);
            }
        }

        #endregion

        #region Booking Retrieval

        public BookingViewModel GetBooking(int id)
        {
            var booking = _bookingRepository.GetBooking(id);
            if (booking == null) return null;

            var vm = MapToViewModel(booking);
            vm.Amenities = GetBookingAmenities(booking.Id);
            return vm;
        }

        public List<BookingViewModel> GetBookings()
        {
            var bookings = _bookingRepository.GetBookings().ToList();
            return bookings.Select(b =>
            {
                var vm = MapToViewModel(b);
                vm.Amenities = GetBookingAmenities(b.Id);
                return vm;
            }).ToList();
        }

        public List<BookingViewModel> GetBookingsByUser(string username)
        {
            var bookings = _bookingRepository.GetBookingsByUser(username).ToList();
            return bookings.Select(b =>
            {
                var vm = MapToViewModel(b);
                vm.Amenities = GetBookingAmenities(b.Id);
                return vm;
            }).ToList();
        }

        public List<BookingViewModel> GetUserBookings(string username) => GetBookingsByUser(username);

        public List<BookingViewModel> GetUpcomingBookingsByUser(string username)
        {
            var bookings = _bookingRepository.GetUpcomingBookingsByUser(username).ToList();
            return bookings.Select(b =>
            {
                var vm = MapToViewModel(b);
                vm.Amenities = GetBookingAmenities(b.Id);
                return vm;
            }).ToList();
        }

        public List<BookingViewModel> GetUpcomingUserBookings(string username) => GetUpcomingBookingsByUser(username);

        public List<BookingViewModel> GetPastBookingsByUser(string username)
        {
            var bookings = _bookingRepository.GetPastBookingsByUser(username).ToList();
            return bookings.Select(b =>
            {
                var vm = MapToViewModel(b);
                vm.Amenities = GetBookingAmenities(b.Id);
                return vm;
            }).ToList();
        }

        public List<BookingViewModel> GetPastUserBookings(string username) => GetPastBookingsByUser(username);

        #endregion

        #region Room & Availability Checks

        public List<BookingViewModel> GetAvailableBookings(DateTime date, string startTime, string endTime)
        {
            var bookings = _bookingRepository.GetBookings()
                .Where(b => b.Date == date && b.StartTime.CompareTo(endTime) < 0 && b.EndTime.CompareTo(startTime) > 0)
                .ToList();

            return bookings.Select(b =>
            {
                var vm = MapToViewModel(b);
                vm.Amenities = GetBookingAmenities(b.Id);
                return vm;
            }).ToList();
        }

        public bool CheckRoomAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null) =>
            _bookingRepository.CheckRoomAvailability(roomId, date, startTime, endTime, excludeBookingId);

        #endregion

        #region Participants

        public List<object> GetAvailableParticipants()
        {
            try
            {
                var bookings = _bookingRepository.GetBookings().ToList();
                var participantDict = new Dictionary<string, object>();

                foreach (var booking in bookings)
                {
                    if (!string.IsNullOrWhiteSpace(booking.Organizer))
                    {
                        var key = booking.Organizer.ToLower();
                        if (!participantDict.ContainsKey(key))
                            participantDict[key] = new { Name = booking.Organizer, UserId = booking.Organizer, Email = "" };
                    }

                    if (booking.Participants != null)
                    {
                        foreach (var participant in booking.Participants)
                        {
                            if (participant != null && !string.IsNullOrWhiteSpace(participant.UserId))
                            {
                                var key = participant.UserId.ToLower();
                                if (!participantDict.ContainsKey(key))
                                    participantDict[key] = new { Name = participant.UserId, UserId = participant.UserId, Email = "" };
                            }
                        }
                    }
                }

                return participantDict.Values.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available participants: {ex.Message}");
                return new List<object>();
            }
        }

        #endregion

        #region Recurring Booking Suggestions

        public async Task<Dictionary<DateTime, List<RoomViewModel>>> GetRecurringRoomSuggestionsAsync(
            BookingViewModel bookingViewModel, List<DateTime> recurrenceDates)
        {
            if (bookingViewModel == null) throw new ArgumentNullException(nameof(bookingViewModel));
            if (recurrenceDates == null || !recurrenceDates.Any())
                return new Dictionary<DateTime, List<RoomViewModel>>();

            var suggestions = await _recurringSuggestionService.GenerateSuggestionsAsync(bookingViewModel, recurrenceDates);

            foreach (var kvp in suggestions)
            {
                _logger.LogInformation($"Recurring suggestions for {kvp.Key.ToShortDateString()}:");
                foreach (var room in kvp.Value)
                    _logger.LogInformation($" - Room {room.RoomName} ({room.MatchedAmenities} amenities matched)");
            }

            return suggestions;
        }

        #endregion

        #region Helpers

        private Booking MapToBooking(BookingViewModel bookingViewModel)
        {
            return new Booking
            {
                RoomId = bookingViewModel.RoomId,
                RoomName = bookingViewModel.RoomName,
                Floor = bookingViewModel.Floor,
                Date = bookingViewModel.Date,
                StartTime = bookingViewModel.StartTime,
                EndTime = bookingViewModel.EndTime,
                Purpose = bookingViewModel.Purpose ?? "Meeting",
                Organizer = bookingViewModel.Organizer ?? "System",
                Recurring = bookingViewModel.Recurring,
                Frequency = bookingViewModel.Frequency,
                RecurringEndDate = bookingViewModel.RecurringEndDate,
                DaysOfWeek = bookingViewModel.DaysOfWeek != null && bookingViewModel.DaysOfWeek.Any()
                    ? string.Join(",", bookingViewModel.DaysOfWeek)
                    : null,
                Image = bookingViewModel.Image ?? "",
                CreatedBy = bookingViewModel.Organizer ?? "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = bookingViewModel.Organizer ?? "System",
                UpdatedTime = DateTime.Now
            };
        }

        private BookingViewModel MapToViewModel(Booking booking)
        {
            return new BookingViewModel
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RoomName = booking.RoomName ?? "",
                Floor = booking.Floor ?? "",
                Date = booking.Date,
                StartTime = booking.StartTime ?? "",
                EndTime = booking.EndTime ?? "",
                Purpose = booking.Purpose ?? "",
                Organizer = booking.Organizer ?? "",
                Recurring = booking.Recurring,
                Frequency = booking.Frequency,
                RecurringEndDate = booking.RecurringEndDate,
                DaysOfWeek = !string.IsNullOrEmpty(booking.DaysOfWeek)
                    ? booking.DaysOfWeek.Split(',').Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => d.Trim()).ToList()
                    : new List<string>(),
                Image = booking.Image ?? "",
                Participants = booking.Participants?.Where(p => p != null).Select(p => p.UserId).ToList() ?? new List<string>(),
                CreatedTime = booking.CreatedTime,
                CreatedBy = booking.CreatedBy ?? "",
                UpdatedTime = booking.UpdatedTime ?? DateTime.Now,
                UpdatedBy = booking.UpdatedBy ?? "",
                Amenities = GetBookingAmenities(booking.Id),
            };
        }

        private void AddBookingParticipants(List<string> participants, int bookingId)
        {
            if (participants == null || !participants.Any()) return;

            foreach (var participantName in participants)
            {
                if (!string.IsNullOrWhiteSpace(participantName))
                    _bookingRepository.AddBookingParticipant(new BookingParticipant
                    {
                        BookingId = bookingId,
                        UserId = participantName
                    });
            }

            _unitOfWork.SaveChanges();
        }

        private void AddBookingAmenities(List<string> amenities, int bookingId)
        {
            // Optional: persist amenities if needed
        }

        private void UpdateBookingAmenities(List<string> amenities, int bookingId)
        {
            // Optional: update persisted amenities if implemented
        }

        private List<string> GetBookingAmenities(int bookingId)
        {
            // Optional: retrieve persisted amenities if implemented
            return new List<string>();
        }

        #endregion
    }
}
