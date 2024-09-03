using CosmeticCatalog.Data;
using CosmeticCatalog.Models;
using CosmeticCatalog.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Runtime.InteropServices;

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


        #region Category

        public async Task<Category?> GetCategoryAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Получить список категорий для построения древовидного меню каталога.
        /// Также отмечает флаги для отображения выбранной категории в меню.
        /// </summary>
        /// <param name="activeCategoryId">Id активного каталога при выборе в меню</param>
        /// <returns></returns>
        public async Task<List<CategoryMenuVM>> GetCategoriesMenuVMAsync(int? activeCategoryId)
        {
            var result = await _context.Categories
                .Select(c => new CategoryMenuVM()
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    HasChildren = c.Children.Count() > 0,
                    HasProducts = c.Products.Count() > 0
                })
                .ToListAsync();

            //Отмечает флаги для построения визуальных отличий элементов меню и его функциональности
            if (activeCategoryId != null && result.Any(c => c.Id == activeCategoryId))
            {
                // Отмечает активную категорию
                var activeCategory = result.FirstOrDefault(c => c.Id == activeCategoryId);
                if (activeCategory == null) return result;
                activeCategory!.IsActive = true;
                if (activeCategory.ParentId != null)
                {
                    bool doFlag = true;
                    CategoryMenuVM? parentCategory = activeCategory;
                    // Проходит по дереву вверх отмечая родительские категории открытыми
                    do
                    {
                        parentCategory = result.FirstOrDefault(c => c.Id == parentCategory.ParentId);
                        if (parentCategory == null) return result;
                        parentCategory.IsOpen = true;
                        if (parentCategory.ParentId == null) doFlag = false;
                    } while (doFlag);
                }
            }

            return result;
        }

        #endregion

        #region Component

        /// <summary>
        /// Получить компонент включая вложенные теги
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Component?> GetComponentAsync(int id)
        {
            return await _context.Components
                .Include(c => c.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Получить полный список компонентов включая вложенные теги
        /// </summary>
        /// <returns></returns>
        public async Task<List<Component>> GetComponentsAsync()
        {
            return await _context.Components
                .Include(c => c.Tags)
                .ToListAsync();
        }

        #endregion

        #region Product

        public async Task<List<Product>> GetSimpleProductListFromCategotyAsync(int? categoryId)
        {
            var result = new List<Product>();
            result = await _context.Products
                .Where(p => p.Category != null && p.Category.Id == categoryId)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
            return result;
        }

        #endregion

        #region Tag

        /// <summary>
        /// Получить полный список тегов
        /// </summary>
        /// <returns></returns>
        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
        }

        /// <summary>
        /// Получить список тегов c помощью списка id тегов
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<Tag>> GetTagsAsync(IEnumerable<int> ids)
        {
            return await _context.Tags.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        #endregion
    }
}
