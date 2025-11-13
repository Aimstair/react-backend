using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookingService
    {
        void AddBooking(BookingViewModel bookingViewModel);
        BookingViewModel GetBooking(int id);
        List<BookingViewModel> GetBookings();
        List<BookingViewModel> GetBookingsByUser(string username);
        List<BookingViewModel> GetUserBookings(string username);
        List<BookingViewModel> GetUpcomingBookingsByUser(string username);
        List<BookingViewModel> GetUpcomingUserBookings(string username);
        List<BookingViewModel> GetPastBookingsByUser(string username);
        List<BookingViewModel> GetPastUserBookings(string username);
        void UpdateBooking(BookingViewModel bookingViewModel);
        void DeleteBooking(int id);
        List<BookingViewModel> GetAvailableBookings(DateTime date, string startTime, string endTime);
        bool CheckRoomAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null);
        List<object> GetAvailableParticipants();
    }
}