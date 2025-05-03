namespace YellowBlossom.Application.DTOs.Project
{
    public class CreateProjectRequest
    {
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
