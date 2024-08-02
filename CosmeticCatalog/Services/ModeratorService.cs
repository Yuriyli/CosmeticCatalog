﻿using CosmeticCatalog.Data;
using CosmeticCatalog.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace CosmeticCatalog.Services
{
    /// <summary>
    /// Сервис для работы модератора с каталогом
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

        /// <summary>
        /// Сохраняет новую категорию в БД
        /// </summary>
        /// <param name="category"></param>
        /// <param name="appUser"></param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> CreateCategory(Category category, AppUser appUser)
        {
            if (category == null || appUser == null)
            {
                _logger.LogError("CreateCategory() Параметр не может быть null");
                return false;
            }

            var mod = new CategoryModification()
            {
                AppUser = appUser,
                DateTime = DateTime.Now,
                ModificationType = ModificationType.Create,
                Info = $"Категория \"{category.Name}\" создана пользователем \"{appUser.UserName}\"",
                Category = category
            };
            category.Modifications.Add(mod);

            try
            {
                await _context.Categories.AddAsync(category);
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
                _logger.LogError($"Не удалось создать новую категорию \"{category.Name}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Поиск категории по id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>Возвращает категорию со всеми модификациями.
        /// Не заполняет список продуктов и вложенных категорий.</returns>
        public async Task<Category?> GetFullCategory(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.Modifications)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        /// <summary>
        /// Обновляет изменения в категории. Для добавления продуктов использовать UpdateProduct(), иначе возникнут проблемы с историей модификаций.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="appUser"></param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> UpdateCategory(Category category, AppUser appUser)
        {
            if (category == null || appUser == null)
            {
                _logger.LogError("UpdateCategory() Параметр не может быть null");
                return false;
            }

            // Проверки на правильность вложений категорий
            if (category.Parent?.Id == category.Id || category.Children.Contains(category))
            {
                _logger.LogError($"UpdateCategory() Ошибка изменения категории \"{category?.Name}\". Категория не может ссылаться на саму себя");
                return false;
            }
            if (category.Parent != null && category.Children.Contains(category.Parent))
            {
                _logger.LogError($"UpdateCategory() Ошибка изменения категории \"{category?.Name}\". " +
                    $"Категория не может содержать один и тот же экземпляр каталога в Parent и Children полях");
                return false;
            }

            var mod = new CategoryModification()
            {
                AppUser = appUser,
                DateTime = DateTime.Now,
                ModificationType = ModificationType.Update,
                Info = $"Категория \"{category.Name}\" изменена пользователем \"{appUser.UserName}\"",
                Category = category
            };
            category.Modifications.Add(mod);

            try
            {
                _context.Categories.Update(category);
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
                _logger.LogError($"Не удалось обновить категорию \"{category.Name}\" Id \"{category.Id}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Удаляет категорию и все модификации. Создает стандартную модификацию с типом Delete
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="appUser"></param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> DeleteCategory(int categoryId, AppUser appUser)
        {
            if (appUser == null)
            {
                _logger.LogError("DeleteCategory() Параметр не может быть null");
                return false;
            }

            var category = await _context.Categories
                .Include(c => c.Children.Take(1))
                .Include(c => c.Products.Take(1))
                .FirstOrDefaultAsync(p => p.Id == categoryId);
            if (category == null)
            {
                _logger.LogError($"Категория с id \"{categoryId}\" не найдена");
                return false;
            }

            // Проверка на наличие зависимых сущностей
            if (category.Children.Count > 0)
            {
                _logger.LogError($"Не удалось удалить категорию \"{category.Name}\" id \"{categoryId}\"." +
                    $" Невозможно удалить категорию если она содержит подкатегории");
                return false;
            }
            if (category.Products.Count > 0)
            {
                _logger.LogError($"Не удалось удалить категорию \"{category.Name}\" id \"{categoryId}\"." +
                    $" Невозможно удалить категорию если она содержит продукты");
                return false;
            }

            try
            {
                var mod = new Modification()
                {
                    AppUser = appUser,
                    DateTime = DateTime.Now,
                    ModificationType = ModificationType.Delete,
                    Info = $"Категория \"{category.Name}\" Id \"{category.Id}\" удален пользователем \"{appUser.UserName}\""
                };

                _context.Remove(category);
                await _context.SaveChangesAsync();

                await _context.Modifications.AddAsync(mod);
                await _context.SaveChangesAsync();

                _logger.LogInformation(mod.Info);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Не удалось удалить категорию \"{category.Name}\" id \"{categoryId}\"");
                _logger.LogError(e.Message);
                return false;
            }
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
                _logger.LogError("CreateProduct() Параметр не может быть null");
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
                .Include(p => p.Components)
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
                _logger.LogError("UpdateProduct() Параметр не может быть null");
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
                _logger.LogError("DeleteProduct() Параметр не может быть null");
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

        /// <summary>
        /// Схраняет новый тег в БД
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="appUser"></param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> CreateTag(Tag tag, AppUser appUser)
        {
            if (tag == null || appUser == null)
            {
                _logger.LogError("CreateTag() Параметр не может быть null");
                return false;
            }

            var mod = new TagModification()
            {
                AppUser = appUser,
                DateTime = DateTime.Now,
                ModificationType = ModificationType.Create,
                Info = $"Тег \"{tag.Name}\" создан пользователем \"{appUser.UserName}\"",
                Tag = tag
            };
            tag.Modifications.Add(mod);

            try
            {
                await _context.Tags.AddAsync(tag);
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
                _logger.LogError($"Не удалось создать новый тег \"{tag.Name}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Поиск тега по Id
        /// </summary>
        /// <param name="TagId"></param>
        /// <returns>Возвращает тег со всеми модификациями. Не заполняет список продуктов и компонентов</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Tag?> GetFullTag(int tagId)
        {
            return await _context.Tags
                .Include(t => t.Modifications)
                .FirstOrDefaultAsync(t => t.Id == tagId);
        }

        /// <summary>
        /// Обновляет изменения в теге. Для добавления тегов в продукты и компоненты - использовать соответсующие методы их обновления.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="appUser"></param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> UpdateTag(Tag tag, AppUser appUser)
        {
            if (tag == null || appUser == null)
            {
                _logger.LogError("UpdateTag() Параметр не может быть null");
                return false;
            }

            if (tag.Components.Count > 0 || tag.Products.Count > 0)
            {
                _logger.LogError($"UpdateTag() Ошибка изменения тега \"{tag?.Name}\". " +
                    $"К тегу невозможно добавить компонент или продукт. Доступны только обратные операции при обновлении компонента или продукта");
                return false;
            }

            var mod = new TagModification()
            {
                AppUser = appUser,
                DateTime = DateTime.Now,
                ModificationType = ModificationType.Update,
                Info = $"Тег \"{tag.Name}\" изменен пользователем \"{appUser.UserName}\"",
                Tag = tag
            };
            tag.Modifications.Add(mod);

            try
            {
                _context.Tags.Update(tag);
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
                _logger.LogError($"Не удалось обновить тег \"{tag.Name}\" Id \"{tag.Id}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Удаляет тег, если нет зависимых продуктов или компонентов
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="appUser"></param>
        /// <returns>bool успех/неуспех</returns>
        public async Task<bool> DeleteTag(int tagId, AppUser appUser)
        {
            if (appUser == null)
            {
                _logger.LogError("DeleteTag() Параметр не может быть null");
                return false;
            }

            var tag = await _context.Tags
                .Include(c => c.Components.Take(1))
                .Include(c => c.Products.Take(1))
                .FirstOrDefaultAsync(p => p.Id == tagId);
            if (tag == null)
            {
                _logger.LogError($"Тег с id \"{tagId}\" не найден");
                return false;
            }

            // Проверка на наличие зависимых сущностей
            if (tag.Components.Count > 0)
            {
                _logger.LogError($"Не удалось удалить тег \"{tag.Name}\" id \"{tagId}\"." +
                    $" Невозможно удалить тег если он используется в компонентах");
                return false;
            }
            if (tag.Products.Count > 0)
            {
                _logger.LogError($"Не удалось удалить тег \"{tag.Name}\" id \"{tagId}\"." +
                    $" Невозможно удалить тег если он используется в  продуктах");
                return false;
            }

            try
            {
                var mod = new Modification()
                {
                    AppUser = appUser,
                    DateTime = DateTime.Now,
                    ModificationType = ModificationType.Delete,
                    Info = $"Тег \"{tag.Name}\" Id \"{tag.Id}\" удален пользователем \"{appUser.UserName}\""
                };

                _context.Remove(tag);
                await _context.SaveChangesAsync();

                await _context.Modifications.AddAsync(mod);
                await _context.SaveChangesAsync();

                _logger.LogInformation(mod.Info);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Не удалось удалить тег \"{tag.Name}\" id \"{tagId}\"");
                _logger.LogError(e.Message);
                return false;
            }
        }

        #endregion
    }
}
