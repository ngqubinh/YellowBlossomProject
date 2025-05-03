namespace YellowBlossom.Application.DTOs.Test
{
    public class CreateTestCaseRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Steps { get; set; } = string.Empty;
        public string ExpetedResult { get; set; } = string.Empty;
        public string ActualResult {  get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid TestTypeId { get; set; }
        public Guid TestCaseStatusId { get; set; }
    }
}
