namespace FoodDeliveryApp.ViewModels
{
    public class AddProductViewModel
    {
        public string ItemName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}