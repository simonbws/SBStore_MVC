using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SBStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Order")]
        [Range(1, 100, ErrorMessage ="Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }

    }
}
