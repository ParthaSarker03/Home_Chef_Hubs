using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryApp.Models
{
    public class Chef
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{5,15}$", ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; } // Changed from int to string

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{5,15}$", ErrorMessage = "Invalid NID format.")]
        public string NID { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}

