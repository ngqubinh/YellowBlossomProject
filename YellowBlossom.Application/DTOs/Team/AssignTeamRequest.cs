namespace YellowBlossom.Application.DTOs.Team
{
    public class AssignTeamRequest
    {
        public string UserId { get; set; } = string.Empty;
        public Guid TeamId { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
