using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRoomRepository
    {
        IQueryable<Room> GetRooms();
        Room GetRoom(int id);
        void AddRoom(Room room);
        void UpdateRoom(Room room);
        void DeleteRoom(Room room);
        bool RoomExists(int id);
        bool RoomNameExists(string roomName, int? excludeId = null);

        void AddRoomAmenity(RoomAmenity roomAmenity);
        void RemoveRoomAmenity(RoomAmenity roomAmenity);
        List<RoomAmenity> GetRoomAmenitiesByRoomId(int roomId);
    }
}