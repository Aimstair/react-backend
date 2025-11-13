using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.Data.Models
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoomName { get; set; }

        [Required]
        [MaxLength(10)]
        public string FloorNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        [MaxLength(500)]
        public string CoverPhoto { get; set; }

        public bool Available { get; set; } = true;

        [Required]
        [MaxLength(50)]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedTime { get; set; }

        // Navigation property for room amenities
        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; }
    }
}