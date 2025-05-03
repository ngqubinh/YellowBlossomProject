using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Team
    {
        private Guid _teamId;
        private string _teamName = string.Empty;
        private string? _teamDescription;
        private string _createdBy = string.Empty;
        private DateTime _createdDate;
        private User? _user;

        // Constructors
        public PMIS_Team() { this._teamId = Guid.NewGuid(); }
        public PMIS_Team(string teamName, string? teamDescription, string createdBy)
        {
            this._teamId = Guid.NewGuid();
            this._teamName = teamName;
            this._teamDescription = teamDescription;
            this._createdBy = createdBy;
            this._createdDate = DateTime.UtcNow;
        }

        // Getters - Setters
        [Key]
        public Guid TeamId { get => this._teamId; private set => this._teamId = Guid.NewGuid(); }
        public string TeamName { get => this._teamName; set => this._teamName = value; }
        public string? TeamDescription { get => this._teamDescription; set => this._teamDescription = value; }
        [Required]
        public string CreatedBy { get => this._createdBy; set => this._createdBy = value; }
        [ForeignKey(nameof(CreatedBy))]
        public User? User { get => this._user; set => this._user = value; }
        public DateTime CreatedDate { get => this._createdDate; set => this._createdDate = DateTime.UtcNow; }

        // Relationships
        public List<PMIS_UserTeam> UserTeams { get; } = [];
        public List<PMIS_ProjectTeam> ProjectTeams { get; } = [];
        public List<PMIS_Task> Tasks { get; } = [];
        public List<PMIS_TestCase> TestCases { get; set; } = new List<PMIS_TestCase>();
        public List<PMIS_TestRun> CreatedTestRuns { get; set; } = new List<PMIS_TestRun>();
        public List<PMIS_TestRun> ExecutedTestRuns { get; set; } = new List<PMIS_TestRun>();
        public List<PMIS_Bug> BugsReportedBy{ get; set; } = new List<PMIS_Bug>();
        public List<PMIS_Bug> BugsAssignedTo { get; set; } = new List<PMIS_Bug>();
    }
}
