using System.ComponentModel.DataAnnotations;
namespace FoodDeliveryApp.ViewModels
{
    public class AdminRegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }


        public string Role { get; set; }
    }
}