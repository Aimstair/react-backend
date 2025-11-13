using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.Data.Models
{
    [Table("Bookings")]
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoomName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Floor { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(10)]
        public string StartTime { get; set; }

        [Required]
        [MaxLength(10)]
        public string EndTime { get; set; }

        [MaxLength(500)]
        public string Purpose { get; set; }

        [MaxLength(100)]
        public string Organizer { get; set; }

        public bool Recurring { get; set; }

        [MaxLength(20)]
        public string Frequency { get; set; }

        public DateTime? RecurringEndDate { get; set; }

        [MaxLength(100)]
        public string DaysOfWeek { get; set; }

        [MaxLength(-1)] 
        public string Image { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }

        [MaxLength(100)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public virtual ICollection<BookingParticipant> Participants { get; set; }
    }
}