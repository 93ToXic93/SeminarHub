using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace SeminarHub.Data.Models
{
    [Comment("Seminars and Participants table")]
    public class SeminarParticipant
    {
        [Required]
        [Comment("Seminar Identifier")]
        public int SeminarId { get; set; }

        [ForeignKey(nameof(SeminarId))]
        public Seminar Seminar { get; set; } = null!;

        [Required]
        [Comment("Participant Identifier")]
        public string ParticipantId { get; set; } = string.Empty;

        [ForeignKey(nameof(ParticipantId))]
        public IdentityUser Participant { get; set; } = null!;

    }
}
