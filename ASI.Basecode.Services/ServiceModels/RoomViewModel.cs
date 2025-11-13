using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string FloorNumber { get; set; }
        public int Capacity { get; set; }
        public string CoverPhoto { get; set; }
        public bool Available { get; set; }
        public List<AmenityViewModel> Amenities { get; set; } = new List<AmenityViewModel>();
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    public class AmenityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}