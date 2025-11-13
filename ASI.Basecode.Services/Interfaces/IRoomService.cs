using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomService
    {
        IEnumerable<RoomViewModel> GetRooms();
        RoomViewModel GetRoom(int id);
        void AddRoom(RoomViewModel room);
        void UpdateRoom(RoomViewModel room);
        void DeleteRoom(int id);
        List<AmenityViewModel> GetAmenities();
    }
}