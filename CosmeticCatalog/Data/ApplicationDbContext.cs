using CosmeticCatalog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CosmeticCatalog.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<Modification> Modifications { get; set; }
        public DbSet<CategoryModification> CategoryModifications { get; set; }
        public DbSet<ComponentModification> ComponentModifications { get; set; }
        public DbSet<ProductModification> ProductModifications { get; set; }
        public DbSet<TagModification> TagModifications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
