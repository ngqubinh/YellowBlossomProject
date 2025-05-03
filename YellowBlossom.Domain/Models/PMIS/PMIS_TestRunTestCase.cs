using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TestRunTestCase
    {
        [Required]
        public Guid TestRunId { get; set; }
        [ForeignKey(nameof(TestRunId))]
        public PMIS_TestRun TestRun { get; set; } = null!;

        [Required]
        public Guid TestCaseId { get; set; }
        [ForeignKey(nameof(TestCaseId))]
        public PMIS_TestCase TestCase { get; set; } = null!;

        public string ExpectedResult { get; set; } = string.Empty;
        public string ActualResult { get; set; } = string.Empty;
        public string ExecutedByUserId = string.Empty;

        [Required]
        public Guid TestCaseStatusId { get; set; }
        [ForeignKey(nameof(TestCaseStatusId))]
        public PMIS_TestCaseStatus? TestCaseStatus { get; set; }

        public Guid? ExecutedByTeamId { get; set; }
        [ForeignKey(nameof(ExecutedByTeamId))]
        public PMIS_Team? ExecutedByTeam { get; set; }

        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
        public int RetryCount { get; set; } = 0;
    }
}
