using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TestRunStatus
    {
        private Guid _testRunStatusId;
        private string _testRunStatusName = string.Empty;

        // Constructors
        public PMIS_TestRunStatus() { this._testRunStatusId = Guid.NewGuid(); }
        public PMIS_TestRunStatus(string name)
        {
            this._testRunStatusId = Guid.NewGuid();
            this._testRunStatusName = name;
        }

        // Getters - Setters
        [Key]
        public Guid TestRunStatusId { get => this._testRunStatusId; private set => this._testRunStatusId = Guid.NewGuid(); }
        public string TestRunStatusName { get => this._testRunStatusName; set => this._testRunStatusName = value; }

        // Relationships
        public List<PMIS_TestRun> TestRuns { get; set; } = new List<PMIS_TestRun>();
    }
}
