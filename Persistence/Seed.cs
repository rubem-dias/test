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
            if (context.Product.Any()) return;
        }
    }
}