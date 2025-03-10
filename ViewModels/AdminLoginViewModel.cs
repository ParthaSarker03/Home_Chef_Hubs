using System.ComponentModel.DataAnnotations;
namespace FoodDeliveryApp.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Password { get; set; }
    }
}