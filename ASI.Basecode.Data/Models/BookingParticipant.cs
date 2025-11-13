using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.Data.Models
{
    [Table("BookingParticipants")]
    public class BookingParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } 

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}