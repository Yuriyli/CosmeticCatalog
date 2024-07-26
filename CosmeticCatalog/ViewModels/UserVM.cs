#nullable disable
namespace CosmeticCatalog.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
