using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_ProjectTeam
    {
        public Guid ProjectId { get; set; }
        public Guid TeamId { get; set; }
        public string RoleOfTeam { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public PMIS_Project Project { get; set; } = null!;
        public PMIS_Team Team { get; set; } = null!;
        [ForeignKey(nameof(CreatedBy))]
        public User? User { get; set; }
    }
}
