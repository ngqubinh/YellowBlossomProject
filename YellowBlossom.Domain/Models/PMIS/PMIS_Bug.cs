using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Bug
    {
        private Guid _bugId;
        private string _title = string.Empty;
        private string? _description;
        private string _stepsToReduce = string.Empty;
        private string _serverity = string.Empty;
        private DateTime _reportedDate;
        private DateTime? _resolvedDate;
        private Guid _priorityId;
        private Guid _testRunId;
        private Guid _testCaseId;
        private Guid _reportedByTeamId;
        private Guid? _assignedToTeamId;
        private PMIS_Priority? _priority;
        private PMIS_TestCase? _testCase;
        private PMIS_TestRun? _testRun;
        private PMIS_Team? _reportedByTeam;
        private PMIS_Team? _assignedToTeam;

        // Constructors
        public PMIS_Bug() { this._bugId = Guid.NewGuid(); }

        // Getters - Setters
        [Key]
        public Guid BugId { get => this._bugId; private set => this._bugId = Guid.NewGuid(); }
        public string Title { get => this._title; set => this._title = value; }
        public string? Description { get => this._description; set => this._description = value; }
        public string StepsToReduce { get => this._stepsToReduce; set => this._stepsToReduce = value; }
        public string Serverity { get => this._serverity; set => this._serverity = value; }
        public DateTime ReportedDate { get => this._reportedDate; set => this._reportedDate = value; }
        public DateTime? ResolvedDate { get => this._resolvedDate; set => this._resolvedDate = value; }
        [Required]
        public Guid PriorityId { get => this._priorityId; set => this._priorityId = value; }
        [ForeignKey(nameof(PriorityId))]
        public PMIS_Priority? Priority { get => this._priority; set => this._priority = value; }
        [Required]
        public Guid TestRunId { get => this._testRunId; set => this._testRunId = value; }
        [ForeignKey(nameof(TestRunId))]
        public PMIS_TestRun? TestRun { get => this._testRun; set => this._testRun = value; }
        [Required]
        public Guid TestCaseId { get => this._testCaseId; set => this._testCaseId = value; }
        [ForeignKey(nameof(TestCaseId))]
        public PMIS_TestCase? TestCase { get => this._testCase; set => this._testCase = value; }
        [Required]
        public Guid ReportedByTeamId { get => this._reportedByTeamId; set => this._reportedByTeamId = value; }
        [ForeignKey(nameof(ReportedByTeamId))]
        public PMIS_Team? ReportedByTeam { get => this._reportedByTeam; set => this._reportedByTeam = value; }
        public Guid? AssignedToTeamId { get => this._assignedToTeamId; set => this._assignedToTeamId = value; }
        [ForeignKey(nameof(AssignedToTeamId))]
        public PMIS_Team? AssignedToTeam { get => this._assignedToTeam; set => this._assignedToTeam = value; }
    }
}
