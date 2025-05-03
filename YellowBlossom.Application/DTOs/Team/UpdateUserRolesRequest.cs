namespace YellowBlossom.Application.DTOs.Team
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
