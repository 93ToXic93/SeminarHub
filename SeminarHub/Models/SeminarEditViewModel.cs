using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.Constants.DataConstants;

namespace SeminarHub.Models
{
    public class SeminarEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredErrorMassage)]
        [StringLength(SeminarTopicMaxLength, MinimumLength = SeminarTopicMinLength, ErrorMessage = ErrorMassage)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMassage)]
        [StringLength(SeminarLecturerMaxLength, MinimumLength = SeminarLecturerMinLength, ErrorMessage = ErrorMassage)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMassage)]
        [StringLength(SeminarDetailsMaxLength, MinimumLength = SeminarDetailsMinLength, ErrorMessage = ErrorMassage)]
        public string Details { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMassage)]
        public string DateAndTime { get; set; } = string.Empty;

        [Range(SeminarDurationMinLength, SeminarDurationMaxLength, ErrorMessage = ErrorMassage)]
        public int? Duration { get; set; }

        [Required(ErrorMessage = RequiredErrorMassage)]
        public int CategoryId { get; set; }

        public string OrganizerId { get; set; } = string.Empty;

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
