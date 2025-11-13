using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IQueryable<Booking> GetBookings()
        {
            return GetDbSet<Booking>().AsQueryable();
        }

        public Booking GetBooking(int id)
        {
            return GetDbSet<Booking>().FirstOrDefault(b => b.Id == id);
        }

        public List<Booking> GetBookingsByUser(string username)
        {
            return GetDbSet<Booking>()
                .Where(b => b.Organizer == username || b.Participants.Any(p => p.UserId == username))
                .ToList();
        }

        public List<Booking> GetUpcomingBookingsByUser(string username)
        {
            var today = DateTime.Today;
            return GetDbSet<Booking>()
                .Where(b => (b.Organizer == username || b.Participants.Any(p => p.UserId == username)) && b.Date >= today)
                .OrderBy(b => b.Date)
                .ToList();
        }

        public List<Booking> GetPastBookingsByUser(string username)
        {
            var today = DateTime.Today;
            return GetDbSet<Booking>()
                .Where(b => (b.Organizer == username || b.Participants.Any(p => p.UserId == username)) && b.Date < today)
                .OrderByDescending(b => b.Date)
                .ToList();
        }


        public void AddBooking(Booking booking)
        {
            GetDbSet<Booking>().Add(booking);
        }

        public void UpdateBooking(Booking booking)
        {
            GetDbSet<Booking>().Update(booking);
        }

        public void DeleteBooking(Booking booking)
        {
            GetDbSet<Booking>().Remove(booking);
        }
        public bool CheckRoomAvailability(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null)
        {
            var query = GetDbSet<Booking>()
                .Where(b => b.RoomId == roomId && b.Date == date);

            if (excludeBookingId.HasValue)
                query = query.Where(b => b.Id != excludeBookingId);

            return !query.Any(b =>
                (b.StartTime.CompareTo(endTime) < 0) &&
                (b.EndTime.CompareTo(startTime) > 0));
        }

        public bool HasConflictingBooking(int roomId, DateTime date, string startTime, string endTime, int? excludeBookingId = null)
        {
            return !CheckRoomAvailability(roomId, date, startTime, endTime, excludeBookingId);
        }

        public void AddBookingParticipant(BookingParticipant participant)
        {
            GetDbSet<BookingParticipant>().Add(participant);
        }

        public void RemoveBookingParticipant(BookingParticipant bookingParticipant)
        {
            this.GetDbSet<BookingParticipant>().Remove(bookingParticipant);
        }

        public List<BookingParticipant> GetBookingParticipantsByBookingId(int bookingId)
        {
            return GetDbSet<BookingParticipant>()
                .Where(p => p.BookingId == bookingId)
                .ToList();
        }
    }
}