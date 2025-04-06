using Microsoft.AspNetCore.Identity;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Domain.Models.Auth
{
    public class User : IdentityUser
    {
        private string _fullName = string.Empty;
        private DateTime _createdAt;

        // Getters - Setters
        public string FullName { get => this._fullName; set => this._fullName = value; }
        public DateTime CreatedAt { get => this._createdAt; set => this._createdAt = DateTime.UtcNow; }

        // Relationships
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public List<Product> Products { get; set;} = new List<Product>();
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
