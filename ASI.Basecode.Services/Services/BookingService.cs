using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class BookingService : ServiceBase, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddBooking(BookingViewModel bookingViewModel)
        {
            try
            {
                var booking = new Booking
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

                _bookingRepository.AddBooking(booking);
                _unitOfWork.SaveChanges();

                if (bookingViewModel.Participants != null && bookingViewModel.Participants.Count > 0)
                {
                    foreach (var participantName in bookingViewModel.Participants)
                    {
                        if (!string.IsNullOrWhiteSpace(participantName))
                        {
                            var bookingParticipant = new BookingParticipant
                            {
                                BookingId = booking.Id,
                                UserId = participantName

                            };

                            _bookingRepository.AddBookingParticipant(bookingParticipant);
                        }
                    }

                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding booking: {ex.Message}", ex.InnerException);
            }
        }

        public BookingViewModel GetBooking(int id)
        {
            var booking = _bookingRepository.GetBooking(id);
            return booking == null ? null : MapToViewModel(booking);
        }

        public List<BookingViewModel> GetBookings()
        {
            var bookings = _bookingRepository.GetBookings().ToList();
            return bookings.Select(MapToViewModel).ToList();
        }

        public List<BookingViewModel> GetBookingsByUser(string username)
        {
            var bookings = _bookingRepository.GetBookingsByUser(username);
            return bookings.Select(MapToViewModel).ToList();
        }

        public List<BookingViewModel> GetUserBookings(string username)
        {
            return GetBookingsByUser(username);
        }

        public List<BookingViewModel> GetUpcomingBookingsByUser(string username)
        {
            var bookings = _bookingRepository.GetUpcomingBookingsByUser(username);
            return bookings.Select(MapToViewModel).ToList();
        }

        public List<BookingViewModel> GetUpcomingUserBookings(string username)
        {
            return GetUpcomingBookingsByUser(username);
        }

        public List<BookingViewModel> GetPastBookingsByUser(string username)
        {
            var bookings = _bookingRepository.GetPastBookingsByUser(username);
            return bookings.Select(MapToViewModel).ToList();
        }

        public List<BookingViewModel> GetPastUserBookings(string username)
        {
            return GetPastBookingsByUser(username);
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
                booking.Recurring = bookingViewModel.Recurring;
                booking.Frequency = bookingViewModel.Frequency;
                booking.RecurringEndDate = bookingViewModel.RecurringEndDate;
                booking.DaysOfWeek = bookingViewModel.DaysOfWeek != null && bookingViewModel.DaysOfWeek.Any() ?
                    string.Join(",", bookingViewModel.DaysOfWeek) : null;
                booking.UpdatedTime = DateTime.Now;
                booking.UpdatedBy = bookingViewModel.Organizer;

                _bookingRepository.UpdateBooking(booking);
                _unitOfWork.SaveChanges();
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

        public List<BookingViewModel> GetAvailableBookings(DateTime date, string startTime, string endTime)
        {
            var bookings = _bookingRepository.GetBookings()
                .Where(b => b.Date == date &&
                       b.StartTime.CompareTo(endTime) < 0 &&
                       b.EndTime.CompareTo(startTime) > 0)
                .ToList();

            return bookings.Select(MapToViewModel).ToList();
        }

        public bool CheckRoomAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null)
        {
            return _bookingRepository.CheckRoomAvailability(roomId, date, startTime, endTime, excludeBookingId);
        }

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
                        {
                            participantDict[key] = new { Name = booking.Organizer, UserId = booking.Organizer, Email = "" };
                        }
                    }

                    if (booking.Participants != null)
                    {
                        foreach (var participant in booking.Participants)
                        {
                            if (participant != null && !string.IsNullOrWhiteSpace(participant.UserId))
                            {
                                var key = participant.UserId.ToLower();
                                if (!participantDict.ContainsKey(key))
                                {
                                    participantDict[key] = new { Name = participant.UserId, UserId = participant.UserId, Email = "" };
                                }
                            }
                        }
                    }
                }

                return participantDict.Values.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available participants: {ex.Message}");
                return new List<object>();
            }
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
                UpdatedBy = booking.UpdatedBy ?? ""
            };
        }
    }
}