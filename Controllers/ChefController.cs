using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using FoodDeliveryApp.ViewModels;

namespace FoodDeliveryApp.Controllers
{
    public class ChefController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string SessionKeyChefEmail = "ChefEmail"; // Session key constant

        public ChefController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Chef/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Chef/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Email and password are required.";
                return View();
            }

            var chef = _context.Chefs.FirstOrDefault(c => c.Email == email);
            if (chef != null && chef.Password == password) // Plain text password check (should hash passwords in production)
            {
                HttpContext.Session.SetString(SessionKeyChefEmail, email);
                return RedirectToAction("ChefIndex");
            }

            ViewBag.ErrorMessage = "Invalid login credentials!";
            return View();
        }

        // GET: Chef/ChefIndex
        public IActionResult ChefIndex()
        {
            if (HttpContext.Session.GetString(SessionKeyChefEmail) == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // GET: Chef/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Chef/SignUp
        [HttpPost]
        public IActionResult SignUp(Chef chef)
        {
            if (_context.Chefs.Any(c => c.Email == chef.Email))
            {
                ViewBag.ErrorMessage = "Email already exists!";
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Chefs.Add(chef); // No password hashing in this example
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage = "Please fix the errors below.";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //////////////////////////////////////////// GET: Chef/AddProduct
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: Chef/AddProduct
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

                return RedirectToAction("ChefIndex");
            }

            return View(model);
        }

        //////////////////////////////////////////// GET: Chef/ViewProducts
        public IActionResult ViewProducts()
        {
            var products = _context.Items.ToList();
            return View(products);
        }



    }
}




//using FoodDeliveryApp.Data;
//using FoodDeliveryApp.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace FoodDeliveryApp.Controllers
//{
//    public class ChefController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private const string SessionKeyChefEmail = "ChefEmail"; // Session key constant

//        public ChefController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Chef/Login
//        public IActionResult Login()
//        {
//            return View();
//        }

//        // POST: Chef/Login
//        [HttpPost]
//        public IActionResult Login(string email, string password)
//        {
//            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
//            {
//                ViewBag.ErrorMessage = "Email and password are required.";
//                return View();
//            }

//            var chef = _context.Chefs.FirstOrDefault(c => c.Email == email);
//            if (chef != null && chef.Password == password) // Plain text password check (should hash passwords in production)
//            {
//                HttpContext.Session.SetString(SessionKeyChefEmail, email);
//                return RedirectToAction("ChefIndex");
//            }

//            ViewBag.ErrorMessage = "Invalid login credentials!";
//            return View();
//        }

//        // GET: Chef/ChefIndex
//        public IActionResult ChefIndex()
//        {
//            if (HttpContext.Session.GetString(SessionKeyChefEmail) == null)
//            {
//                return RedirectToAction("Login");
//            }

//            return View();
//        }

//        // GET: Chef/SignUp
//        public IActionResult SignUp()
//        {
//            return View();
//        }

//        // POST: Chef/SignUp
//        [HttpPost]
//        public IActionResult SignUp(Chef chef)
//        {
//            if (_context.Chefs.Any(c => c.Email == chef.Email))
//            {
//                ViewBag.ErrorMessage = "Email already exists!";
//                return View();
//            }

//            if (ModelState.IsValid)
//            {
//                _context.Chefs.Add(chef); // No password hashing in this example
//                _context.SaveChanges();
//                return RedirectToAction("Login");
//            }

//            ViewBag.ErrorMessage = "Please fix the errors below.";
//            return View();
//        }

//        // Logout
//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login");
//        }
//    }
//}

