using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FoodDeliveryApp.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime OrderDateTime { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        // Total price for the entire order
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation property to all items in this order
        public List<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }
    }
}