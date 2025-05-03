namespace YellowBlossom.Application.DTOs.General
{
    public class UserTeamDTO
    {
        public string UserId { get; set; } = string.Empty;
        public Guid TeamId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string? Message { get; set; }
    }
}
