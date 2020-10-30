using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsAuthorised { get; set; }
    }
}
