using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SBStoreWebRazor_Temp.Models
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
        [Range(1, 100)]
        public int DisplayOrder { get; set; }

    }
}
