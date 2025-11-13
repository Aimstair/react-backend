using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string Floor { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Purpose { get; set; }
        public string Organizer { get; set; }
        public bool Recurring { get; set; }
        public string Frequency { get; set; }
        public DateTime? RecurringEndDate { get; set; }
        public List<string> DaysOfWeek { get; set; }
        public string Image { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public List<string> Participants { get; set; } 
    }
}