using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Application.DTOs.Test
{
    public class UpdateTestRunTestCaseRequest
    {
        [Required(ErrorMessage = "The actualResult field is required.")]
        public string ActualResult { get; set; } = string.Empty;
        public Guid TestCaseStatusId { get; set; }
    }
}
