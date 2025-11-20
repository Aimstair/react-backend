using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookingService
    {
        void AddBooking(BookingViewModel bookingViewModel);
        void UpdateBooking(BookingViewModel bookingViewModel);
        void DeleteBooking(int id);

        BookingViewModel GetBooking(int id);
        List<BookingViewModel> GetBookings();
        List<BookingViewModel> GetBookingsByUser(string username);
        List<BookingViewModel> GetUpcomingBookingsByUser(string username);
        List<BookingViewModel> GetPastBookingsByUser(string username);

        List<BookingViewModel> GetAvailableBookings(DateTime date, string startTime, string endTime);
        bool CheckRoomAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null);

        List<object> GetAvailableParticipants();
        Task<Dictionary<DateTime, List<RoomViewModel>>> GetRecurringRoomSuggestionsAsync(
            BookingViewModel bookingViewModel,
            List<DateTime> recurrenceDates);
    }
}
