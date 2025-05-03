using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TestCase
    {
        private Guid _testCaseId;
        private string _title = string.Empty;
        private string? _description;
        private string _steps = string.Empty;
        private string _expectedResult = string.Empty;
        private string _actualResult = string.Empty;
        private Guid _taskId;
        private Guid _createdBy;
        private Guid _testTypeId;
        private Guid _testCaseStatusId;
        private PMIS_Task? _task;
        private PMIS_Team? _team;
        private PMIS_TestCaseStatus? _testCaseStatus;
        private PMIS_TestType? _testType;

        // Constructors
        public PMIS_TestCase() { this._testCaseId = Guid.NewGuid(); }

        // Getters - Setters
        [Key]
        public Guid TestCaseId { get => this._testCaseId; private set => this._testCaseId = Guid.NewGuid(); }
        public string Title { get => this._title; set => this._title = value; }
        public string? Description { get => this._description; set => this._description = value; }
        public string Steps { get => this._steps; set => this._steps = value; }
        public string ExpectedResult { get => this._expectedResult; set => this._expectedResult = value; }
        public string ActualResult { get => this._actualResult; set => this._actualResult = value; }
        [Required]
        public Guid TaskId { get => this._taskId; set => this._taskId = value; }
        [ForeignKey(nameof(TaskId))]
        public PMIS_Task? Task { get => this._task ; set => this._task = value; }
        [Required]
        public Guid CreateBy { get => this._createdBy; set => this._createdBy = value; }
        [ForeignKey(nameof(CreateBy))]
        public PMIS_Team? Team { get => this._team; set => this._team = value; }
        [Required]
        public Guid TestTypeId { get => this._testTypeId;  set => this._testTypeId = value; }
        [ForeignKey(nameof(TestTypeId))]
        public PMIS_TestType? TestType { get => this._testType; set => this._testType = value; }
        [Required]
        public Guid TestCaseStatusId { get => this._testCaseStatusId; set => this._testCaseStatusId = value; }
        [ForeignKey(nameof(TestCaseStatusId))]
        public PMIS_TestCaseStatus? TestCaseStatus { get => this._testCaseStatus; set => this._testCaseStatus = value; }

        // Relationships
        public List<PMIS_TestRunTestCase> TestRunTestCases { get; set; } = [];
        public List<PMIS_Bug> Bugs { get; set; } = new List<PMIS_Bug>();
    }
}
