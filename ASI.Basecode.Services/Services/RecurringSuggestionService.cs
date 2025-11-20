using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class RecurringSuggestionService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<RecurringSuggestionService> _logger;

        public RecurringSuggestionService(
            IRoomRepository roomRepository,
            IBookingRepository bookingRepository,
            ILogger<RecurringSuggestionService> logger)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public async Task<Dictionary<DateTime, List<RoomViewModel>>> GenerateSuggestionsAsync(
            BookingViewModel originalBooking,
            List<DateTime> recurrenceDates)
        {
            var suggestions = new Dictionary<DateTime, List<RoomViewModel>>();

            var allRooms = _roomRepository.GetRooms().ToList();
            var allBookings = _bookingRepository.GetBookings().ToList();

            var floors = allRooms
                .Select(r => int.TryParse(r.FloorNumber, out int f) ? f : -1)
                .Where(f => f >= 0)
                .Distinct()
                .ToList();

            foreach (var date in recurrenceDates)
                suggestions[date] = FindRoomsForDate(originalBooking, date, allRooms, allBookings, floors);

            return suggestions;
        }

        #region Helpers

        private List<RoomViewModel> FindRoomsForDate(
            BookingViewModel originalBooking,
            DateTime date,
            List<Room> allRooms,
            List<Booking> allBookings,
            List<int> floors)
        {
            var dailySuggestions = new List<RoomViewModel>();
            var usedRoomIds = new HashSet<int>();
            int originalFloor = int.TryParse(originalBooking.Floor, out int fNum) ? fNum : 0;

            // Step 1: Original room
            var originalRoom = allRooms.FirstOrDefault(r => r.Id == originalBooking.RoomId);
            if (originalRoom != null && IsRoomAvailable(originalRoom, date, originalBooking, allBookings))
            {
                var matched = GetMatchedAmenities(originalBooking, originalRoom);
                dailySuggestions.Add(MapRoomToViewModel(originalRoom, matched));
                usedRoomIds.Add(originalRoom.Id);
            }

            // Step 2: Exact matches
            var floorOffsets = new List<int> { 0, 1, -1, 2, -2, 3, -3 };
            foreach (var offset in floorOffsets)
            {
                int floorToCheck = originalFloor + offset;
                if (!floors.Contains(floorToCheck)) continue;

                var candidateRooms = allRooms
                    .Where(r => int.TryParse(r.FloorNumber, out int f) && f == floorToCheck && !usedRoomIds.Contains(r.Id))
                    .ToList();

                foreach (var room in candidateRooms)
                {
                    if (!IsRoomAvailable(room, date, originalBooking, allBookings)) continue;
                    var matched = GetMatchedAmenities(originalBooking, room);
                    if (matched.Count == (originalBooking.Amenities?.Count ?? 0))
                    {
                        dailySuggestions.Add(MapRoomToViewModel(room, matched));
                        usedRoomIds.Add(room.Id);
                    }
                }
            }

            // Step 3: Partial matches
            if (dailySuggestions.Count == 0)
            {
                foreach (var offset in floorOffsets)
                {
                    int floorToCheck = originalFloor + offset;
                    if (!floors.Contains(floorToCheck)) continue;

                    var candidateRooms = allRooms
                        .Where(r => int.TryParse(r.FloorNumber, out int f) && f == floorToCheck && !usedRoomIds.Contains(r.Id))
                        .ToList();

                    foreach (var room in candidateRooms)
                    {
                        if (!IsRoomAvailable(room, date, originalBooking, allBookings)) continue;
                        var matched = GetMatchedAmenities(originalBooking, room);
                        if (matched.Count > 0)
                        {
                            dailySuggestions.Add(MapRoomToViewModel(room, matched));
                            usedRoomIds.Add(room.Id);
                        }
                    }

                    if (dailySuggestions.Count > 0) break;
                }
            }

            return dailySuggestions.OrderByDescending(r => r.MatchedAmenities).ToList();
        }

        private bool IsRoomAvailable(Room room, DateTime date, BookingViewModel originalBooking, List<Booking> allBookings)
        {
            return !allBookings.Any(b =>
                b.RoomId == room.Id &&
                b.Date == date &&
                (b.StartTime.CompareTo(originalBooking.EndTime) < 0 &&
                 b.EndTime.CompareTo(originalBooking.StartTime) > 0));
        }

        private List<string> GetMatchedAmenities(BookingViewModel originalBooking, Room room)
        {
            if (originalBooking.Amenities == null || !originalBooking.Amenities.Any()) return new List<string>();

            var roomAmenityNames = room.RoomAmenities?.Select(ra => ra.Amenity.Name).ToList() ?? new List<string>();
            return originalBooking.Amenities.Where(a => roomAmenityNames.Contains(a)).ToList();
        }

        private RoomViewModel MapRoomToViewModel(Room room, List<string> matchedAmenities)
        {
            return new RoomViewModel
            {
                Id = room.Id,
                RoomName = room.RoomName,
                FloorNumber = room.FloorNumber,
                Capacity = room.Capacity,
                CoverPhoto = room.CoverPhoto,
                MatchedAmenities = matchedAmenities.Count,
                MatchedAmenityNames = matchedAmenities,
                Amenities = room.RoomAmenities?.Select(ra => new AmenityViewModel
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name,
                    Description = ra.Amenity.Description,
                    IsActive = ra.Amenity.IsActive
                }).ToList() ?? new List<AmenityViewModel>()
            };
        }

        #endregion
    }
}
