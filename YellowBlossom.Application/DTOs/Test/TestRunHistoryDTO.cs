namespace YellowBlossom.Application.DTOs.Test
{
    public class TestRunHistoryDTO
    {
        public Guid TestRunId { get; set; }
        public Guid TestCaseId { get; set; }
        public string ActualResult { get; set; } = string.Empty;
        public string TestCaseStatus { get; set; } = string.Empty;
        public string ExecutedByTeam { get; set; } = string.Empty;
        public DateTime ExecutedAt { get; set; }
        public string? Message {  get; set; }
    }

}
