using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using FoodDeliveryApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FoodDeliveryApp.Controllers
{
    [Authorize] // Ensures only logged-in users can access this controller
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult MakePayment(int orderId, decimal amount)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return RedirectToAction("Login", "CustomerAccount");
            }
            int customerId = int.Parse(customerIdClaim.Value);

            var order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.OrderId == orderId && o.CustomerId == customerId);

            if (order == null)
            {
                return NotFound("Order not found or does not belong to the customer.");
            }

            var payment = new Payment
            {
                OrderId = orderId,
                Method = PaymentMethod.CashOnDelivery, // Default method, can be modified
                Amount = amount,
                PaymentDateTime = DateTime.Now
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            return RedirectToAction("PaymentSuccess", new { orderId = orderId });
        }

        public IActionResult PaymentSuccess(int orderId)
        {
            var payment = _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Customer)
                .Include(p => p.Order.OrderItems) // Ensure OrderItems are loaded
                .FirstOrDefault(p => p.OrderId == orderId);

            if (payment == null || payment.Order == null || payment.Order.Customer == null)
            {
                return NotFound("Payment or Order details not found.");
            }

            var totalQuantity = payment.Order.OrderItems?.Sum(oi => oi.Quantity) ?? 0; // Prevent null exception

            var model = new PaymentViewModel
            {
                OrderId = payment.OrderId,
                CustomerName = payment.Order.Customer.CustomerName,
                PhoneNumber = payment.Order.Customer.PhoneNumber,
                Address = payment.Order.Customer.Address,
                Method = payment.Method,
                TotalQuantity = totalQuantity,
                TotalPrice = payment.Amount,
                PaymentDateTime = payment.PaymentDateTime
            };

            return View(model);
        }
    }
}
