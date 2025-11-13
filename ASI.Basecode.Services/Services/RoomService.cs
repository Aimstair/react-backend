using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IRoomRepository roomRepository, IAmenityRepository amenityRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _amenityRepository = amenityRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<RoomViewModel> GetRooms()
        {
            var rooms = _roomRepository.GetRooms();
            return rooms.Select(r => new RoomViewModel
            {
                Id = r.Id,
                RoomName = r.RoomName,
                FloorNumber = r.FloorNumber,
                Capacity = r.Capacity,
                CoverPhoto = r.CoverPhoto,
                Available = r.Available,
                Amenities = r.RoomAmenities.Select(ra => new AmenityViewModel
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name,
                    Description = ra.Amenity.Description,
                    IsActive = ra.Amenity.IsActive
                }).ToList(),
                CreatedBy = r.CreatedBy,
                CreatedTime = r.CreatedTime,
                UpdatedBy = r.UpdatedBy,
                UpdatedTime = r.UpdatedTime
            });
        }

        public RoomViewModel GetRoom(int id)
        {
            var room = _roomRepository.GetRoom(id);
            if (room == null) return null;

            return new RoomViewModel
            {
                Id = room.Id,
                RoomName = room.RoomName,
                FloorNumber = room.FloorNumber,
                Capacity = room.Capacity,
                CoverPhoto = room.CoverPhoto,
                Available = room.Available,
                Amenities = room.RoomAmenities.Select(ra => new AmenityViewModel
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name,
                    Description = ra.Amenity.Description,
                    IsActive = ra.Amenity.IsActive
                }).ToList(),
                CreatedBy = room.CreatedBy,
                CreatedTime = room.CreatedTime,
                UpdatedBy = room.UpdatedBy,
                UpdatedTime = room.UpdatedTime
            };
        }

        public void AddRoom(RoomViewModel roomViewModel)
        {
            if (_roomRepository.RoomNameExists(roomViewModel.RoomName))
            {
                throw new InvalidOperationException("Room name already exists");
            }

            var room = new Data.Models.Room // Assuming 'Data.Models' is the namespace for your DB models
            {
                RoomName = roomViewModel.RoomName?.Trim(),
                FloorNumber = roomViewModel.FloorNumber?.Trim(),
                Capacity = roomViewModel.Capacity,
                CoverPhoto = roomViewModel.CoverPhoto?.Trim() ?? "",
                Available = roomViewModel.Available,
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now,
                RoomAmenities = new List<RoomAmenity>() // Initialize the navigation property
            };

            _roomRepository.AddRoom(room);
            _unitOfWork.SaveChanges(); // Save to get the room ID

            // Add amenities after room is saved (to get room ID)
            if (roomViewModel.Amenities?.Any() == true)
            {
                foreach (var amenity in roomViewModel.Amenities)
                {
                    var roomAmenity = new RoomAmenity
                    {
                        RoomId = room.Id,
                        AmenityId = amenity.Id // Use the ID from the AmenityViewModel
                    };
                    _roomRepository.AddRoomAmenity(roomAmenity); // Use repository method
                }
                _unitOfWork.SaveChanges(); // Save amenities
            }
        }

        // Inside RoomService.cs, ensure the UpdateRoom method looks like this:
        public void UpdateRoom(RoomViewModel roomViewModel)
        {
            var room = _roomRepository.GetRoom(roomViewModel.Id);
            if (room == null)
            {
                throw new InvalidOperationException("Room not found");
            }

            if (_roomRepository.RoomNameExists(roomViewModel.RoomName, roomViewModel.Id))
            {
                throw new InvalidOperationException("Room name already exists");
            }

            // Update room properties
            room.RoomName = roomViewModel.RoomName?.Trim();
            room.FloorNumber = roomViewModel.FloorNumber?.Trim();
            room.Capacity = roomViewModel.Capacity;
            room.CoverPhoto = roomViewModel.CoverPhoto?.Trim() ?? "";
            room.Available = roomViewModel.Available;
            room.UpdatedBy = "System";
            room.UpdatedTime = DateTime.Now;

            // Remove existing amenities for this room
            var existingRoomAmenities = _roomRepository.GetRoomAmenitiesByRoomId(room.Id);
            foreach (var ra in existingRoomAmenities)
            {
                _roomRepository.RemoveRoomAmenity(ra); // Use repository method
            }

            // Add new amenities
            if (roomViewModel.Amenities?.Any() == true)
            {
                foreach (var amenity in roomViewModel.Amenities)
                {
                    var roomAmenity = new RoomAmenity
                    {
                        RoomId = room.Id,
                        AmenityId = amenity.Id // Use the ID from the AmenityViewModel
                    };
                    _roomRepository.AddRoomAmenity(roomAmenity); // Use repository method
                }
            }

            _roomRepository.UpdateRoom(room);
            _unitOfWork.SaveChanges();
        }

        public void DeleteRoom(int id)
        {
            var room = _roomRepository.GetRoom(id);
            if (room == null)
            {
                throw new InvalidOperationException("Room not found");
            }

            _roomRepository.DeleteRoom(room);
            _unitOfWork.SaveChanges();
        }

        public List<AmenityViewModel> GetAmenities()
        {
            var amenities = _amenityRepository.GetActiveAmenities();
            return amenities.Select(a => new AmenityViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                IsActive = a.IsActive
            }).ToList();
        }
    }
}