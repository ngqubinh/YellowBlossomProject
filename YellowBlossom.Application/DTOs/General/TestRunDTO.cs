namespace YellowBlossom.Application.DTOs.General
{
    public class TestRunDTO
    {
        public Guid TestRunId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime RunDate { get; set; }
        public Guid TaskId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ExecutedBy { get; set; }
        public Guid TestRunStatusId { get; set; }
        public TaskDTO? Task { get; set; }
        public TeamDTO? CreatedByTeam { get; set; }
        public TeamDTO? ExecutedByTeam { get; set; }
        public TestRunStatusDTO? TestRunStatus { get; set; }
        public string? Message { get; set; }
    }
}
