namespace FoodDeliveryApp.ViewModels
{
    public class AdminOrdersViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public List<string> Items { get; set; } // e.g., "Pizza x1 ($9.99)"
        public decimal TotalPrice { get; set; }
        public string OrderDateTime { get; set; }
    }
}