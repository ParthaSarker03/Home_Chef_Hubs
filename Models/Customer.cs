using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace FoodDeliveryApp.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        // Navigation property: a Customer can have multiple Orders
        public List<Order> Orders { get; set; }
    }
}