using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ASI.Basecode.Services.ServiceModels
{
    public partial class BookingViewModel : IValidatableObject
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "RoomId is required")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "RoomName is required")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Floor is required")]
        public string Floor { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "StartTime is required")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "EndTime is required")]
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

    public partial class BookingViewModel
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            // Ensure date is valid (not default)
            if (this.Date == default)
            {
                errors.Add(new ValidationResult("Date is invalid", new[] { nameof(this.Date) }));
            }

            // Validate StartTime / EndTime ordering. Expect formats like HH:mm or h:mm tt.
            if (!string.IsNullOrWhiteSpace(this.StartTime) && !string.IsNullOrWhiteSpace(this.EndTime))
            {
                if (TimeSpan.TryParse(this.StartTime, out var start) && TimeSpan.TryParse(this.EndTime, out var end))
                {
                    if (start >= end)
                    {
                        errors.Add(new ValidationResult("StartTime must be before EndTime", new[] { nameof(this.StartTime), nameof(this.EndTime) }));
                    }
                }
                else
                {
                    errors.Add(new ValidationResult("StartTime or EndTime have invalid format", new[] { nameof(this.StartTime), nameof(this.EndTime) }));
                }
            }

            return errors;
        }
    }
}