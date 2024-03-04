using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SBStoreWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Order")]
        public int DisplayOrder { get; set; }

    }
}
