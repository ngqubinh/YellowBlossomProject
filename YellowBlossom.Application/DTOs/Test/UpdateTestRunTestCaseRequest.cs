using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Application.DTOs.Test
{
    public class UpdateTestRunTestCaseRequest
    {
        public string ActualResult { get; set; } = string.Empty;
        public Guid TestCaseStatusId { get; set; }
    }
}
