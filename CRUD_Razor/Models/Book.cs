using System.ComponentModel.DataAnnotations;

namespace CRUD_Razor.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter a book title.")]
        [Display(Name = "Title")]
        [StringLength(50)]
        public string? title { get; set; }

        [Required(ErrorMessage = "Enter the author's name.")]
        [Display(Name = "Author")]
        [StringLength(50)]
        public string? author { get; set; }

        [Display(Name = "Description")]
        [StringLength(50)]
        public string? description { get; set; }

        [Display(Name = "Publisher")]
        [StringLength(50)]
        public string? publisher { get; set; }

        [Required]
        [Display(Name = "Status")]
        [RegularExpression("^(Available|Issued)$", ErrorMessage = "Choose Available or Issued.")]
        [StringLength(50)]
        public string? status { get; set; } = "Available";

        [Display(Name = "Category")]
        [StringLength(50)]
        public string? category { get; set; }
    }
}
