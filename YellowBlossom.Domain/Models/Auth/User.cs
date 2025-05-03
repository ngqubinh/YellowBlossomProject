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
        public List<PMIS_Product> Products { get; set;} = new List<PMIS_Product>();
        public List<PMIS_Project> Projects { get; set; } = new List<PMIS_Project>();
        public List<PMIS_Task> Tasks { get; set; } = new List<PMIS_Task>();
        public List<PMIS_UserTeam> UserTeams { get; } = [];
    }
}
