using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBStore.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public int Name { get; set; }

        public int? StreetAddress { get; set; }
        public int? City { get; set; }
        public int? State { get; set; }
        public int? PostalCode { get; set; }
    }
}
