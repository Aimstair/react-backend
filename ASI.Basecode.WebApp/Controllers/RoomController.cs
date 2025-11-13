using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: api/Room/GetRooms
        [HttpGet("GetRooms")]
        [AllowAnonymous]
        public IActionResult GetRooms()
        {
            try
            {
                var rooms = _roomService.GetRooms();
                // Map RoomViewModels to anonymous objects or a DTO for the API response
                var roomList = rooms.Select(r => new
                {
                    id = r.Id,
                    roomName = r.RoomName,
                    floorNumber = r.FloorNumber,
                    capacity = r.Capacity,
                    // Map AmenityViewModels to their names or IDs for the response
                    amenities = r.Amenities.Select(a => new { id = a.Id, name = a.Name }).ToArray(), // Send objects for frontend
                    coverPhoto = r.CoverPhoto, // Send the cover photo string (either path or data URL)
                    available = r.Available
                });
                return Ok(roomList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get rooms: {ex.Message}");
            }
        }

        // GET: api/Room/GetAmenities
        [HttpGet("GetAmenities")]
        [AllowAnonymous]
        public IActionResult GetAmenities()
        {
            try
            {
                var amenities = _roomService.GetAmenities();
                var amenityList = amenities.Select(a => new
                {
                    id = a.Id,
                    name = a.Name,
                    description = a.Description
                });
                return Ok(amenityList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get amenities: {ex.Message}");
            }
        }

        // POST: api/Room/AddRoom
        [HttpPost("AddRoom")]
        [AllowAnonymous]
        public IActionResult AddRoom([FromBody] AddRoomRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RoomName))
                {
                    return BadRequest(new { message = "Room name is required" });
                }
                if (string.IsNullOrEmpty(request.FloorNumber))
                {
                    return BadRequest(new { message = "Floor number is required" });
                }
                if (request.Capacity <= 0)
                {
                    return BadRequest(new { message = "Capacity must be greater than 0" });
                }

                // Get amenity objects from amenity IDs
                var allAmenities = _roomService.GetAmenities();
                var selectedAmenities = new List<AmenityViewModel>();
                if (request.Amenities?.Any() == true)
                {
                    foreach (var amenityId in request.Amenities)
                    {
                        var amenity = allAmenities.FirstOrDefault(a => a.Id == amenityId);
                        if (amenity != null)
                        {
                            selectedAmenities.Add(amenity);
                        }
                        else
                        {
                            return BadRequest(new { message = $"Amenity with ID {amenityId} not found." });
                        }
                    }
                }

                // Process coverPhoto if needed (e.g., save to disk and store path)
                // For now, pass the data URL string directly
                var coverPhotoString = request.CoverPhoto ?? "";

                var newRoom = new RoomViewModel
                {
                    RoomName = request.RoomName,
                    FloorNumber = request.FloorNumber,
                    Capacity = request.Capacity,
                    CoverPhoto = coverPhotoString, // Pass the string (data URL or path)
                    Available = true,
                    Amenities = selectedAmenities
                };

                _roomService.AddRoom(newRoom);

                return Ok(new { message = "Room added successfully" });
            }
            catch (Exception ex)
            {
                // Log the full exception details here for debugging
                Console.WriteLine($"AddRoom Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { message = $"Failed to add room: {ex.Message}" });
            }
        }

        // PUT: api/Room/UpdateRoom/{id}
        [HttpPut("UpdateRoom/{id}")]
        [AllowAnonymous]
        public IActionResult UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RoomName))
                {
                    return BadRequest(new { message = "Room name is required" });
                }
                if (string.IsNullOrEmpty(request.FloorNumber))
                {
                    return BadRequest(new { message = "Floor number is required" });
                }
                if (request.Capacity <= 0)
                {
                    return BadRequest(new { message = "Capacity must be greater than 0" });
                }

                // Get amenity objects from amenity IDs
                var allAmenities = _roomService.GetAmenities();
                var selectedAmenities = new List<AmenityViewModel>();
                if (request.Amenities?.Any() == true)
                {
                    foreach (var amenityId in request.Amenities)
                    {
                        var amenity = allAmenities.FirstOrDefault(a => a.Id == amenityId);
                        if (amenity != null)
                        {
                            selectedAmenities.Add(amenity);
                        }
                        else
                        {
                            return BadRequest(new { message = $"Amenity with ID {amenityId} not found." });
                        }
                    }
                }

                // Process coverPhoto if needed (e.g., save to disk and store path)
                // For now, pass the data URL string directly
                var coverPhotoString = request.CoverPhoto ?? "";

                var updatedRoom = new RoomViewModel
                {
                    Id = id,
                    RoomName = request.RoomName,
                    FloorNumber = request.FloorNumber,
                    Capacity = request.Capacity,
                    CoverPhoto = coverPhotoString, // Pass the string (data URL or path)
                    Available = request.Available,
                    Amenities = selectedAmenities
                };

                _roomService.UpdateRoom(updatedRoom);

                return Ok(new { message = "Room updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the full exception details here for debugging
                Console.WriteLine($"UpdateRoom Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { message = $"Failed to update room: {ex.Message}" });
            }
        }

        // DELETE: api/Room/DeleteRoom/{id}
        [HttpDelete("DeleteRoom/{id}")]
        [AllowAnonymous]
        public IActionResult DeleteRoom(int id)
        {
            try
            {
                _roomService.DeleteRoom(id);
                return Ok(new { message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to delete room: {ex.Message}" });
            }
        }
    }

    // Define the request models
    public class AddRoomRequest
    {
        public string RoomName { get; set; }
        public string FloorNumber { get; set; }
        public int Capacity { get; set; }
        public string CoverPhoto { get; set; } // Expecting data URL string
        public List<int> Amenities { get; set; } // Expecting List of Amenity IDs
    }

    public class UpdateRoomRequest
    {
        public string RoomName { get; set; }
        public string FloorNumber { get; set; }
        public int Capacity { get; set; }
        public string CoverPhoto { get; set; } // Expecting data URL string
        public bool Available { get; set; }
        public List<int> Amenities { get; set; } // Expecting List of Amenity IDs
    }
}