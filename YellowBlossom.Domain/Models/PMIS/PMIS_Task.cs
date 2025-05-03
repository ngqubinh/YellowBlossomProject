using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Task
    {
        private Guid _taskId; 
        private string _title = string.Empty;
        private string _description = string.Empty;
        private DateTime _startDate;
        private DateTime _endDate;
        private Guid _priorityId;
        private Guid _taskStatusId;
        private Guid _projectId;
        private string _createdBy = string.Empty;
        private Guid? _assignedTeam;
        private PMIS_Priority? _priority;
        private PMIS_TaskStatus? _taskStatus;
        private PMIS_Project? _project;
        private User? _user;
        private PMIS_Team? _team;

        // Constructors
        public PMIS_Task() { this._taskId = Guid.NewGuid(); }

        // Getters - Setters
        [Key]
        public Guid TaskId { get => this._taskId; private set => this._taskId = Guid.NewGuid(); }
        public string Title { get => this._title; set => this._title = value; }
        public string Description { get => this._description; set => this._description = value; }
        public DateTime StartDate { get => this._startDate; set => this._startDate = value; }
        public DateTime EndDate { get => this._endDate; set => this._endDate = value; }
        [Required]
        public Guid PriorityId { get => this._priorityId; set => this._priorityId = value; }
        [ForeignKey(nameof(PriorityId))]
        public PMIS_Priority? Priority { get => this._priority; set => this._priority = value; }
        [Required]
        public Guid TaskStatusId { get => this._taskStatusId; set => this._taskStatusId = value; }
        [ForeignKey(nameof(PriorityId))]
        public PMIS_TaskStatus? TaskStatus { get => this._taskStatus; set => this._taskStatus = value; }
        [Required]
        public Guid ProjectId { get => this._projectId; set => this._projectId = value; }
        [ForeignKey(nameof(ProjectId))]
        public PMIS_Project? Project { get => this._project; set => this._project = value; }
        [Required]
        public string CreatedBy { get => this._createdBy; set => this._createdBy = value; }
        [ForeignKey(nameof(CreatedBy))]
        public User? User { get => this._user; set => this._user = value; }
        public Guid? AssignedTeam { get => this._assignedTeam; set => this._assignedTeam = value; }
        [ForeignKey(nameof(AssignedTeam))]
        public PMIS_Team? Team { get => this._team; set => this._team = value; }

        // Relationships
        public List<PMIS_TestCase> TestCases { get; set; } = new List<PMIS_TestCase>();
        public List<PMIS_TestRun> TestRuns { get; set; } = new List<PMIS_TestRun>();
        public List<PMIS_Bug> Bugs { get; set; } = new List<PMIS_Bug>();
    }
}
