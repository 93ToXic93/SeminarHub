using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static SeminarHub.Data.Constants.DataConstants;

namespace SeminarHub.Data.Models
{
    [Comment("Seminar's table")]
    public class Seminar
    {
        [Key]
        [Comment("Seminar Identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(SeminarTopicMaxLength)]
        [Comment("Seminar's topic")]
        public string Topic { get; set; } = string.Empty;

        [Required] 
        [MaxLength(SeminarLecturerMaxLength)]
        [Comment("The lecturer of the seminar")]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [MaxLength(SeminarDetailsMaxLength)]
        [Comment("Details about the seminar")]
        public string Details { get; set; } = string.Empty;

        [Required]
        [Comment("Organizer Identifier")]
        public string OrganizerId { get; set; } = string.Empty;

        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        [Comment("The date and time for the seminar")]
        public DateTime DateAndTime { get; set; }

        [Range(SeminarDurationMinLength,SeminarDurationMaxLength)]
        [Comment("Duration of the seminar")]
        public int? Duration { get; set; }

        [Required]
        [Comment("Category Identifier")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public ICollection<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();

    }
}

