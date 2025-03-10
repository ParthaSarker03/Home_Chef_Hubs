using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using FoodDeliveryApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApp.Controllers
{
    [Authorize] // or [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Customer/PlaceOrder
        // Displays all items with Quantity fields in one form
        [HttpGet]
        public IActionResult PlaceOrder()
        {
            // 1) Check CustomerId claim
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return RedirectToAction("Login", "CustomerAccount");
            }

            // 2) Get only available items
            var items = _context.Items
                .AsNoTracking()
                .Where(i => i.IsAvailable)
                .ToList();

            // 3) Map items to the new view model
           

            var model = new MultipleItemOrderViewModel
            {
                Items = items.Select(i => new OrderItemViewModel
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Price = i.Price,
                    IsAvailable = i.IsAvailable,
                    Quantity = 0,
                    ImagePath = i.ImagePath
                }).ToList()
            };
               return View(model);

        // 4) Return the view with the form

        }

        // POST: /Customer/PlaceOrder
        // Processes the selected items and creates a single Order with multiple OrderItems
        [HttpPost]
        public IActionResult PlaceOrder(MultipleItemOrderViewModel model)
        {
            // 1) Verify the user is still authenticated
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return RedirectToAction("Login", "CustomerAccount");
            }
            int customerId = int.Parse(customerIdClaim.Value);

            // 2) Filter out items with Quantity > 0
            var selectedItems = model.Items.Where(x => x.Quantity > 0).ToList();

            if (!selectedItems.Any())
            {
                ModelState.AddModelError("", "No items selected. Please pick at least one item.");
                return View(model); // Re-display the form
            }

            // 3) Create a new order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDateTime = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };

            // 4) For each selected item, create an OrderItem
            foreach (var sel in selectedItems)
            {
                // 1) Look up the real item from the DB
                var dbItem = _context.Items.FirstOrDefault(i => i.ItemId == sel.ItemId && i.IsAvailable);

                if (dbItem == null)
                {
                    // The item was not found or not available
                    continue; // or handle error
                }

                // 2) Use the real, current price from DB
                var orderItem = new OrderItem
                {
                    ItemId = dbItem.ItemId,
                    Quantity = sel.Quantity,
                    UnitPrice = dbItem.Price
                };

                order.OrderItems.Add(orderItem);
            }

            // 5) Calculate total Price
            order.Price = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            // 6) Save
            _context.Orders.Add(order);
            _context.SaveChanges();

            // 7) Redirect to MyOrders
            return RedirectToAction("MyOrders");
        }

        public IActionResult MyOrders()
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return RedirectToAction("Login", "CustomerAccount");
            }
            int customerId = int.Parse(customerIdClaim.Value);

            // Get all orders for this customer
            var orders = _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDateTime)
                .ToList();

            var customerOrders = new List<CustomerOrderViewModel>();

            foreach (var order in orders)
            {
                var orderVm = new CustomerOrderViewModel
                {
                    OrderId = order.OrderId,
                    OrderTotalPrice = order.Price,
                    Status = order.Status.ToString(),
                    OrderDateTime = order.OrderDateTime.ToString("g"),
                    Items = new List<CustomerOrderItemViewModel>()
                };

                // Add each item in the order
                foreach (var oi in order.OrderItems)
                {
                    orderVm.Items.Add(new CustomerOrderItemViewModel
                    {
                        ItemName = oi.Item.ItemName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    });
                }

                customerOrders.Add(orderVm);
            }

            return View(customerOrders);
        }
    }
}