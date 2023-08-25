using Domain.Models;
using Domain.Enum;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Product.Any()) return;
            
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Celular",
                    Price = 1000,
                    Category = Category.Lightweight
                },
                new Product
                {
                    Name = "Notebook",
                    Price = 3000,
                    Category = Category.AverageWeight
                },
                new Product
                {
                    Name = "Televisão",
                    Price = 5000,
                    Category = Category.Heavy
                },
            };

            await context.Product.AddRangeAsync(products);
            await context.SaveChangesAsync();

            if (context.Customer.Any()) return;
            var customers = new List<Customer>
            {
                new Customer
                {
                    Id = "20297271059",
                    Name = "Cliente1"
                },
                new Customer
                {
                    Id = "51739008065",
                    Name = "Cliente2"
                },
                new Customer
                {
                    Id = "03232015042",
                    Name = "Cliente3"
                },
                new Customer
                {
                    Id = "09983602016",
                    Name = "Cliente4"
                },
                new Customer
                {
                    Id = "76420735009",
                    Name = "Cliente5"
                },
                new Customer
                {
                    Id = "62729349049",
                    Name = "Cliente6"
                },
                new Customer
                {
                    Id = "07088731037",
                    Name = "Cliente7"
                }
            };

            await context.Customer.AddRangeAsync(customers);
            await context.SaveChangesAsync();

        }
    }
}