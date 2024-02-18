using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data.Constants;

namespace SeminarHub.Data.Models
{
    [Comment("Category's table")]
    public class Category
    {
        [Key]
        [Comment("Category Identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.CategoryNameMaxLength)]
        [Comment("Category name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Seminar> Seminars { get; set; } = new List<Seminar>();
    }
}
