namespace YellowBlossom.Application.DTOs.General
{
    public class TestRunTestCaseDTO
    {
        public Guid TestRunId { get; set; }
        public Guid TestCaseId { get; set; }
        public string ActualResult { get; set; } = string.Empty;
        public Guid TestCaseStatusId { get; set; }
        public TestRunDTO TestRun { get; set; } = null!;
        public TestCaseDTO TestCase { get; set; } = null!;
        public TestCaseStatusDTO? TestCaseStatus { get; set; }
        public string? Message { get; set; }
    }
}
