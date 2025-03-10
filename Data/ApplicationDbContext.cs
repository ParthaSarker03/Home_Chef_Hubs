using FoodDeliveryApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodDeliveryApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Chef> Chefs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminId = 1, Name = "SuperAdmin", Email = "admin@example.com", Password = "admin@123", PhoneNumber = "1234567890", Role = "SuperAdmin" }
            );

            // Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "John Doe", Email = "john@example.com", Password = "john@123", PhoneNumber = "9876543210", Address = "123 Main Street" },
                new Customer { CustomerId = 2, CustomerName = "Jane Smith", Email = "jane@example.com", Password = "jane@123", PhoneNumber = "5554443322", Address = "456 South Avenue" }
            );

            // Items
            modelBuilder.Entity<Item>().HasData(
                new Item { ItemId = 1, ItemName = "Margherita Pizza", Description = "Classic cheese pizza", Price = 9.99m, IsAvailable = true },
                new Item { ItemId = 2, ItemName = "Veggie Burger", Description = "Delicious burger with veggie patty", Price = 7.49m, IsAvailable = true },
                new Item { ItemId = 3, ItemName = "Pepperoni Pizza", Description = "Pizza with pepperoni toppings", Price = 10.99m, IsAvailable = true },
                new Item { ItemId = 4, ItemName = "Chicken Burger", Description = "Juicy chicken burger", Price = 8.49m, IsAvailable = true },
                new Item { ItemId = 5, ItemName = "French Fries", Description = "Crispy golden fries", Price = 3.49m, IsAvailable = true }
            );

            // Orders
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, CustomerId = 1, OrderDateTime = DateTime.Today.AddMinutes(10), Status = OrderStatus.Pending, Price = 9.99m },  // Single item #1
                new Order { OrderId = 2, CustomerId = 2, OrderDateTime = DateTime.Today.AddMinutes(25), Status = OrderStatus.Accepted, Price = 7.49m },  // Single item #2
                new Order { OrderId = 3, CustomerId = 1, OrderDateTime = DateTime.Today.AddMinutes(32), Status = OrderStatus.Accepted, Price = 10.98m }, // Items #2, #5
                new Order { OrderId = 4, CustomerId = 2, OrderDateTime = DateTime.Today.AddMinutes(45), Status = OrderStatus.Pending, Price = 10.99m },  // Single item #3
                new Order { OrderId = 5, CustomerId = 2, OrderDateTime = DateTime.Today.AddMinutes(45), Status = OrderStatus.Rejected, Price = 10.98m }, // Items #2, #5
                new Order { OrderId = 6, CustomerId = 1, OrderDateTime = DateTime.Today.AddMinutes(50), Status = OrderStatus.Rejected, Price = 10.99m }  // Single item #3
            );

            // OrderItems
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { OrderItemId = 1, OrderId = 1, ItemId = 1, Quantity = 1, UnitPrice = 9.99m },
                new OrderItem { OrderItemId = 2, OrderId = 2, ItemId = 2, Quantity = 1, UnitPrice = 7.49m },
                new OrderItem { OrderItemId = 3, OrderId = 3, ItemId = 2, Quantity = 1, UnitPrice = 7.49m },
                new OrderItem { OrderItemId = 4, OrderId = 3, ItemId = 5, Quantity = 1, UnitPrice = 3.49m },
                new OrderItem { OrderItemId = 5, OrderId = 4, ItemId = 3, Quantity = 1, UnitPrice = 10.99m },
                new OrderItem { OrderItemId = 6, OrderId = 5, ItemId = 2, Quantity = 1, UnitPrice = 7.49m },
                new OrderItem { OrderItemId = 7, OrderId = 5, ItemId = 5, Quantity = 1, UnitPrice = 3.49m },
                new OrderItem { OrderItemId = 8, OrderId = 6, ItemId = 3, Quantity = 1, UnitPrice = 10.99m }
            );
            // Payments
           

        }
    }
}