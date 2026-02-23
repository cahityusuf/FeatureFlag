using FeatureFlag.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlag.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var products = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
        return View(products);
    }
}
