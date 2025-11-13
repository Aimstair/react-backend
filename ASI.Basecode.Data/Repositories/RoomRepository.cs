using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IQueryable<Room> GetRooms()
        {
            return this.GetDbSet<Room>()
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity);
        }

        public Room GetRoom(int id)
        {
            return this.GetDbSet<Room>()
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity)
                .FirstOrDefault(x => x.Id == id);
        }

        public void AddRoom(Room room)
        {
            this.GetDbSet<Room>().Add(room);
        }

        public void UpdateRoom(Room room)
        {
            this.GetDbSet<Room>().Update(room);
        }

        public void DeleteRoom(Room room)
        {
            this.GetDbSet<Room>().Remove(room);
        }

        public bool RoomExists(int id)
        {
            return this.GetDbSet<Room>().Any(x => x.Id == id);
        }

        public bool RoomNameExists(string roomName, int? excludeId = null)
        {
            var query = this.GetDbSet<Room>().Where(x => x.RoomName.ToLower() == roomName.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            return query.Any();
        }

        public void AddRoomAmenity(RoomAmenity roomAmenity)
        {
            this.GetDbSet<RoomAmenity>().Add(roomAmenity);
        }

        public void RemoveRoomAmenity(RoomAmenity roomAmenity)
        {
            this.GetDbSet<RoomAmenity>().Remove(roomAmenity);
        }

        public void RemoveRoomAmenitiesByRoomId(int roomId)
        {
            var existingAmenities = this.GetDbSet<RoomAmenity>().Where(ra => ra.RoomId == roomId);
            this.GetDbSet<RoomAmenity>().RemoveRange(existingAmenities);
        }

        public List<RoomAmenity> GetRoomAmenitiesByRoomId(int roomId)
        {
            return Context.Set<RoomAmenity>()
                .Where(ra => ra.RoomId == roomId)
                .ToList();
        }

    }
}