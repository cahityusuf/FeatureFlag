using FeatureFlag.Models;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlag.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Products.AnyAsync())
            return;

        var products = new[]
        {
            new Product { Name = "Laptop", Description = "Yüksek performanslı dizüstü bilgisayar", Price = 24999.99m, Stock = 15 },
            new Product { Name = "Klavye", Description = "Mekanik RGB klavye", Price = 899.00m, Stock = 50 },
            new Product { Name = "Mouse", Description = "Kablosuz oyun faresi", Price = 549.00m, Stock = 75 },
            new Product { Name = "Monitör", Description = "27 inç 4K monitör", Price = 8999.00m, Stock = 20 },
            new Product { Name = "Kulaklık", Description = "Gürültü önleyici kulaklık", Price = 1299.00m, Stock = 40 },
            new Product { Name = "Webcam", Description = "1080p HD web kamerası", Price = 699.00m, Stock = 30 },
            new Product { Name = "SSD 1TB", Description = "NVMe M.2 SSD", Price = 2499.00m, Stock = 100 },
            new Product { Name = "RAM 16GB", Description = "DDR4 3200MHz bellek", Price = 1199.00m, Stock = 60 }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
