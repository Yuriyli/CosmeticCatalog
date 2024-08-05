using CosmeticCatalog.Data;
using CosmeticCatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmeticCatalog.Services
{
    public class CatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly ApplicationDbContext _context;

        public CatalogService(ILogger<CatalogService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Product>> GetSimpleProductListFromCategotyAsync(int? categoryId)
        {
            var result = new List<Product>();
            result = await _context.Products
                .Where(p => p.Category!=null && p.Category.Id == categoryId)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
            return result;
        }

        public async Task<List<Category>> GetCategoriesForMenuAsync()
        {
            return await _context.Categories
                .Include(c => c.Parent)
                .ToListAsync();
        }
    }
}
