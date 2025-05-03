namespace YellowBlossom.Application.DTOs.Team
{
    public class CreateTeamRequest
    {
        public string TeamName { get; set; } = string.Empty;
        public string? Description {  get; set; }
    }
}
