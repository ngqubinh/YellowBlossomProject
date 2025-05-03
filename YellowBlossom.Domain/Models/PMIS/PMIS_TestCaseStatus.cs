using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TestCaseStatus
    {
        private Guid _testCaseStatusId;
        private string _testCaseStatusName = string.Empty;

        // Constructors
        public PMIS_TestCaseStatus() { this._testCaseStatusId = Guid.NewGuid(); }
        public PMIS_TestCaseStatus(string testCaseStatusName)
        {
            this._testCaseStatusId= Guid.NewGuid();
            this._testCaseStatusName = testCaseStatusName;
        }

        // Getters - Setters
        [Key]
        public Guid TestCaseStatusId { get => this._testCaseStatusId; private set => this._testCaseStatusId = Guid.NewGuid(); }
        public string TestCaseStatusName { get => this._testCaseStatusName; set => this._testCaseStatusName = value; }

        // Relationships
        public List<PMIS_TestCase> TestCases { get; set; } = new List<PMIS_TestCase>();
    }
}
