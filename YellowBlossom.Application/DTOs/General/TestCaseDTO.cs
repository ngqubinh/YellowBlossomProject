namespace YellowBlossom.Application.DTOs.General
{
    public class TestCaseDTO
    {
        public Guid TestCaseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Steps { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
        public string ActualResult {  get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid TestTypeId { get; set; }
        public Guid TestCaseStatusId { get; set; }
        public TaskDTO? Task { get; set; }
        public TeamDTO? Team { get; set; }
        public TestTypeDTO? TestType { get; set; }
        public TestCaseStatusDTO? TestCaseStatus { get; set; }
        public string? Message { get; set; }
        public List<BugDTO> Bugs { get; set; } = new List<BugDTO>();
    }
}
