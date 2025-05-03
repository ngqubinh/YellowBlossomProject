using System.ComponentModel.DataAnnotations;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_UserTeam
    {
        public string UserId { get; set; } = string.Empty;
        public Guid TeamId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        public PMIS_Team Team { get; set; } = null!;
    }
}
