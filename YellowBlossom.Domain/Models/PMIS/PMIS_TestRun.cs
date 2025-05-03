using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TestRun
    {
        private Guid _testRunId;
        private string _title = string.Empty;
        private string? _description;
        private DateTime _runDate;
        private Guid _taskId;
        private Guid _createdBy;
        private Guid? _executedBy;
        private Guid _testRunStatusId;
        private PMIS_Task? _task;
        private PMIS_Team? _createdByTeam;
        private PMIS_Team? _executedByTeam;
        private PMIS_TestRunStatus? _testRunStatus;

        // Constructors
        public PMIS_TestRun() { this._testRunId = Guid.NewGuid(); }

        // Getters - Setters
        [Key]
        public Guid TestRunId { get => this._testRunId; private set => this._testRunId = value; }
        public string Title { get => this._title; set => this._title = value; }
        public string? Description { get => this._description; set => this._description = value; }
        public DateTime RunDate { get => this._runDate; set => this._runDate = value > DateTime.UtcNow ? DateTime.UtcNow : value; }

        [Required]
        public Guid TaskId { get => this._taskId; set => this._taskId = value; }
        [ForeignKey(nameof(TaskId))]
        public PMIS_Task? Task { get => this._task; set => this._task = value; }

        public Guid CreatedBy { get => this._createdBy; set => this._createdBy = value; }
        [ForeignKey(nameof(CreatedBy))]
        public PMIS_Team? CreatedByTeam{ get => this._createdByTeam; set => this._createdByTeam = value; }

        public Guid? ExecutedBy { get => this._executedBy; set => this._executedBy = value; }
        [ForeignKey(nameof(ExecutedBy))]
        public PMIS_Team? ExecutedByTeam { get => this._executedByTeam; set => this._executedByTeam = value; }

        [Required]
        public Guid TestRunStatusId { get => this._testRunStatusId; set => this._testRunStatusId = value; }
        [ForeignKey(nameof(TestRunStatusId))]
        public PMIS_TestRunStatus? TestRunStatus { get => this._testRunStatus; set => this._testRunStatus = value; }

        // Relationships
        public List<PMIS_TestRunTestCase> TestRunTestCases { get; set; } = [];
        public List<PMIS_Bug> Bugs { get; set; } = new List<PMIS_Bug>();
    }
}
