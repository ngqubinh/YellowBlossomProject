using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Application.DTOs.General
{
    public class ProjectTeamDTO
    {
        public Guid ProjectId { get; set; }
        public Guid TeamId { get; set; }
        public string RoleOfTeam { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public ProjectDTO? Project { get; set; } = null!;
        public UserDTO? User { get; set; }
        public string? Message { get; set; }
    }
}
