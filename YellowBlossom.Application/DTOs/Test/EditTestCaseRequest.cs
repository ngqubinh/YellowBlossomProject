namespace YellowBlossom.Application.DTOs.Test
{
    public class EditTestCaseRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Steps { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
        public string ActualResult {  get; set; } = string.Empty;
        public Guid TestTypeId { get; set; }
        public Guid TestCaseStatusId { get; set; }
    }
}
