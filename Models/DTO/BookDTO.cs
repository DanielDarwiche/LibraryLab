using System.ComponentModel.DataAnnotations;

namespace LibraryLab.Models.DTO
{
    public class BookDTO
    {
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int YearOfPublication { get; set; }
        public string Description { get; set; }
        public bool AvailableToBorrow { get; set; }

    }
}
