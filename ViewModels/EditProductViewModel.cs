using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryApp.ViewModels
{
    public class EditProductViewModel
    {
        public int ItemId { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
