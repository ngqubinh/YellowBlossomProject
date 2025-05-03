namespace YellowBlossom.Application.DTOs.Bug
{
    public class EditBugRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public string StepToReduce { get; set; } = string.Empty;
        public string Severity {  get; set; } = string.Empty;
        public Guid PriorityId { get; set; }
    }
}
