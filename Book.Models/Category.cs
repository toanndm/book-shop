using System.ComponentModel.DataAnnotations;

namespace Book.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [Range(0, 100, ErrorMessage = "DisplayOrder must be between 0 and 100")]
        public int DisplayOrder { get; set; }
    }
}
