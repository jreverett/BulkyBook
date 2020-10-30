using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
