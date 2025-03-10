using FoodDeliveryApp.Models;

namespace FoodDeliveryApp.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public PaymentMethod Method { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDateTime { get; set; }
    }
}