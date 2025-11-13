using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetBookings();
        Booking GetBooking(int id);
        List<Booking> GetBookingsByUser(string username);
        List<Booking> GetUpcomingBookingsByUser(string username);
        List<Booking> GetPastBookingsByUser(string username);
        void AddBooking(Booking booking);
        void UpdateBooking(Booking booking);
        void DeleteBooking(Booking booking);
        void AddBookingParticipant(BookingParticipant participant);
        List<BookingParticipant> GetBookingParticipantsByBookingId(int bookingId);
        void RemoveBookingParticipant(BookingParticipant participant);
        bool CheckRoomAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null);
    }
}