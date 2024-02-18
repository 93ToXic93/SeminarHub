namespace SeminarHub.Models
{
    public class SeminarDetailsViewModel : SeminarAllViewModel
    {
        public string Details { get; set; } = string.Empty;

        public int? Duration { get; set; }

    }
}
