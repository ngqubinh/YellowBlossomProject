using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Invite
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public PMIS_Project? Project { get; set; }

        public string? InvitedUserId { get; set; }

        [ForeignKey(nameof(InvitedUserId))]
        public User? InvitedUser { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }
    }
}