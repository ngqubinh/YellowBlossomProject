namespace YellowBlossom.Application.DTOs.Test
{
    public class CreateTestRunRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime RunDate { get; set; }
        public Guid TaskId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ExecutedBy { get; set; }
        public Guid TestRunStatusId { get; set; }
    }
}
