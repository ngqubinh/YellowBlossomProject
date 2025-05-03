namespace YellowBlossom.Application.DTOs.Test
{
    public class EditTestRunRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid TestRunStatusId { get; set; }
    }
}
