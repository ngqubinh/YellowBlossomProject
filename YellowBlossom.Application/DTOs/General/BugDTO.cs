namespace YellowBlossom.Application.DTOs.General
{
    public class BugDTO
    {
        public Guid BugId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string StepsToReduces { get; set; } = string.Empty;
        public string Serverity { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public Guid PriorityId { get; set; }
        public Guid TestRunId { get; set; }
        public Guid TestCaseId { get; set; }
        public Guid ReportedByTeamId { get; set; }
        public Guid? AssginedToTeamId { get; set; }
        public PriorityDTO? Priority { get; set; }
        public TestRunDTO? TestRun { get; set; }
        public TestCaseDTO? TestCase { get; set; }
        public TeamDTO? ReportedByTeam { get; set; }
        public TeamDTO? AssignedToTeam { get; set; }
        public string? Message { get; set; }
    }
}
