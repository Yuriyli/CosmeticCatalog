using CosmeticCatalog.Data;
using CosmeticCatalog.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace CosmeticCatalog.Services
{
    /// <summary>
    /// Сервис для работы с каталогом
    /// </summary>
    public class ModeratorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModeratorService> _logger;

        public ModeratorService(ApplicationDbContext context, ILogger<ModeratorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region [Category]

        public async Task<bool> CreateCategory(Category category, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<Category> GetFullCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateCategory(Category category, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteCategory(int categoryId, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [Component]

        public async Task<bool> CreateComponent(Component component, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<Component> GetFullComponent(int componentId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateComponent(Component component, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteComponent(int componentId, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [Product]

        /// <summary>
        /// Сохраняет новый Product в БД
        /// </summary>
        /// <param name="product">Новая сущность продукта</param>
        /// <param name="appUser">AppUser для сохранения историй модификации</param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> CreateProduct(Product product, AppUser appUser)
        {
            if (product == null || appUser == null)
            {
                _logger.LogError("Параметр не может быть null");
                return false;
            }

            var mod = new ProductModification()
            {
                AppUser = appUser,
                DateTime = DateTime.Now,
                ModificationType = ModificationType.Create,
                Info = $"Продукт \"{product.Name}\" создан пользователем \"{appUser.UserName}\"",
                Product = product
            };
            product.Modifications.Add(mod);

            try
            {
                await _context.Products.AddAsync(product);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation(mod.Info);
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Не удалось создать новый продукт \"{product.Name}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Получить продукт из БД со всеми модификациями
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>Продукт или null</returns>
        public async Task<Product?> GetFullProduct(int productId)
        {
            var result = await _context.Products
                .Include(p => p.Tags)
                .Include(p => p.Modifications)
                .FirstOrDefaultAsync(p => p.Id == productId);
            return result;
        }

        /// <summary>
        /// Изменяет существующий Product
        /// </summary>
        /// <param name="product">Продукт, который нужно изменить</param>
        /// <param name="appUser">AppUser для сохранения историй модификации</param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> UpdateProduct(Product product, AppUser appUser)
        {
            if (product == null || appUser == null)
            {
                _logger.LogError("Параметр не может быть null");
                return false;
            }

            var mod = new ProductModification()
            {
                AppUser = appUser,
                DateTime = DateTime.Now,
                ModificationType = ModificationType.Update,
                Info = $"Продукт \"{product.Name}\" Id \"{product.Id}\" изменен пользователем \"{appUser.UserName}\"",
                Product = product
            };
            product.Modifications.Add(mod);

            try
            {
                _context.Products.Update(product);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    _logger.LogInformation(mod.Info);
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Не удалось обновить продукт \"{product.Name}\" Id \"{product.Id}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Удаляет продукт и историю модификаций, создает после удаления базовый класс Modification
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProduct(int productId, AppUser appUser)
        {
            if (appUser == null)
            {
                _logger.LogError("Параметр не может быть null");
                return false;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                _logger.LogError($"Продукт с id \"{productId}\" не найден");
                return false;
            }

            try
            {
                var mod = new Modification()
                {
                    AppUser = appUser,
                    DateTime = DateTime.Now,
                    ModificationType = ModificationType.Delete,
                    Info = $"Продукт \"{product.Name}\" Id \"{product.Id}\" удален пользователем \"{appUser.UserName}\""
                };

                _context.Remove(product);
                await _context.SaveChangesAsync();

                await _context.Modifications.AddAsync(mod);
                await _context.SaveChangesAsync();

                _logger.LogInformation(mod.Info);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Не удалось удалить продукт \"{product.Name}\" id \"{productId}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        #endregion

        #region [Tag]

        public async Task<bool> CreateTag(Tag tag, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<Tag> GetFullTag(int TagId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTag(Tag tag, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteTag(int tagId, AppUser appUser)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
