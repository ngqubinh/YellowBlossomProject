namespace YellowBlossom.Application.DTOs.Team
{
    public class InvitationDTO
    {
        public Guid Id { get; set; }
        public string InvitedEmail { get; set; }
        public Guid TeamId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
