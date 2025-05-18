using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_InviteW
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public Guid TeamId { get; set; } // Đổi từ ProjectId sang TeamId

        [ForeignKey(nameof(TeamId))]
        public PMIS_Team? Team { get; set; } // Thêm reference đến Team

        [Required]
        public string InvitedEmail { get; set; } // Email người được mời

        public string? InvitedUserId { get; set; } // UserId (nếu đã tồn tại)

        [ForeignKey(nameof(InvitedUserId))]
        public User? InvitedUser { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending; // Thay bool bằng enum
    }

    public enum InvitationStatus
    {
        Pending,
        Accepted,
        Expired
    }
}