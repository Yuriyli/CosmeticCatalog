using Microsoft.AspNetCore.Identity;

namespace CosmeticCatalog.Models
{
    public class AppUser : IdentityUser
    {
        public List<Modification> Modifications { get; set; } = new();
    }
}
