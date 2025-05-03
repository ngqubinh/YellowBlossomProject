namespace YellowBlossom.Application.DTOs.Test
{
    public class TestSummaryDTO
    {
        public int TotalTestCases { get; set; }
        public int TotalTestRuns { get; set; }
        public int ExecutedTestCases { get; set; }
        public int FailedTestCases { get; set; }
        public string? Message { get; set; }
    }
}
