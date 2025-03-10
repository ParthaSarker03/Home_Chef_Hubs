using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using FoodDeliveryApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApp.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admin role can access
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Products
        public IActionResult Products()
        {
            // If you want the AdminId from the claims (optional):
            // var adminIdClaim = User.FindFirst("AdminId");
            // int adminId = adminIdClaim != null ? int.Parse(adminIdClaim.Value) : 0;

            // Fetch items (AsNoTracking for read-only)
            var items = _context.Items
                .AsNoTracking()
                .ToList();

            return View(items);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var newItem = new Item
                {
                    ItemName = model.ItemName,
                    Description = model.Description,
                    Price = model.Price,
                    IsAvailable = model.IsAvailable,
                    ImagePath = "/images/" + uniqueFileName
                };

                _context.Items.Add(newItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Products");
            }

            return View(model);
        }


        // GET: /Admin/Orders
        public IActionResult Orders()
        {
            // If you need the admin ID:
            // var adminIdClaim = User.FindFirst("AdminId");
            // int adminId = adminIdClaim != null ? int.Parse(adminIdClaim.Value) : 0;

            // AutoRejectOldPendingOrders();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Fetch only today's orders AsNoTracking
            var ordersToday = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
                .Where(o => o.OrderDateTime >= today && o.OrderDateTime < tomorrow)
                .OrderByDescending(o => o.OrderDateTime)
                .ToList();

            var pendingOrders = ordersToday.Where(o => o.Status == OrderStatus.Pending);
            var acceptedOrders = ordersToday.Where(o => o.Status == OrderStatus.Accepted);
            var rejectedOrders = ordersToday.Where(o => o.Status == OrderStatus.Rejected);

            // Convert to ViewModels
            ViewBag.PendingOrders = MapToAdminOrdersViewModel(pendingOrders);
            ViewBag.AcceptedOrders = MapToAdminOrdersViewModel(acceptedOrders);
            ViewBag.RejectedOrders = MapToAdminOrdersViewModel(rejectedOrders);

            return View();
        }

        // POST: /Admin/AcceptOrder
        [HttpPost]
        public IActionResult AcceptOrder(int orderId)
        {
            // No more session checks - [Authorize(Roles="Admin")] ensures only admins can reach here
            var order = _context.Orders.Find(orderId);
            if (order != null && order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.Accepted;
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }

        // POST: /Admin/RejectOrder
        [HttpPost]
        public IActionResult RejectOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null && order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.Rejected;
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }

        private void AutoRejectOldPendingOrders()
        {
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            var oldPendingOrders = _context.Orders
                .Where(o => o.Status == OrderStatus.Pending && o.OrderDateTime < fiveMinutesAgo)
                .ToList();

            foreach (var ord in oldPendingOrders)
            {
                ord.Status = OrderStatus.Rejected;
            }
            _context.SaveChanges();
        }

        // Maps Orders to AdminOrdersViewModel
        private List<AdminOrdersViewModel> MapToAdminOrdersViewModel(IEnumerable<Order> orders)
        {
            var result = new List<AdminOrdersViewModel>();
            foreach (var o in orders)
            {
                var itemsList = o.OrderItems.Select(oi =>
                    $"{oi.Item.ItemName} x{oi.Quantity} (${oi.UnitPrice})").ToList();

                result.Add(new AdminOrdersViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.CustomerName,
                    CustomerEmail = o.Customer.Email,
                    Items = itemsList,
                    TotalPrice = o.Price,
                    OrderDateTime = o.OrderDateTime.ToString("g")
                });
            }
            return result;
        }
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            var model = new EditProductViewModel
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                ImagePath = item.ImagePath
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var item = _context.Items.Find(model.ItemId);
            if (item == null)
            {
                return NotFound();
            }

            // Handle image upload if a new image is provided
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                // Update the image path
                item.ImagePath = "/images/" + uniqueFileName;
            }

            // Update other fields
            item.ItemName = model.ItemName;
            item.Description = model.Description;
            item.Price = model.Price;
            item.IsAvailable = model.IsAvailable;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Products");
        }

        public IActionResult PaymentInfo()
        {
            var paymentInfo = _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Customer)
                .Include(p => p.Order.OrderItems)
                .Select(payment => new PaymentViewModel
                {
                    OrderId = payment.OrderId,
                    CustomerName = payment.Order.Customer.CustomerName,
                    PhoneNumber = payment.Order.Customer.PhoneNumber,
                    Address = payment.Order.Customer.Address,
                    Method = payment.Method,
                    TotalQuantity = payment.Order.OrderItems.Sum(oi => oi.Quantity),
                    TotalPrice = payment.Amount,
                    PaymentDateTime = payment.PaymentDateTime
                })
                .ToList(); // <-- Ensure it's a list

            return View(paymentInfo);
        }


    }
}