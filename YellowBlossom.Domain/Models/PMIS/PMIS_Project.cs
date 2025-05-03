using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Project
    {
        private Guid _projectId;
        private string _projectName = string.Empty;
        private string? _description;
        private DateTime _startDate;
        private DateTime _endDate;
        private Guid _projectStatusId;
        private PMIS_ProjectStatus? _projectStatus;
        private Guid _productId;
        private PMIS_Product? _product;
        private string _userId = string.Empty;
        private User? _user;
        private Guid _projectTypeId;
        private PMIS_ProjectType? _projectType;
        private string _productManager = string.Empty;

        // Constructors
        public PMIS_Project() { }
        public PMIS_Project(string projectName, string description, DateTime endDate, Guid projectStatusId, Guid productId, string userId, Guid projectTypeId, string pm)
        {
            this._projectId = Guid.NewGuid();
            this._projectName = projectName;
            this._description = description;
            this._startDate = DateTime.UtcNow;
            this._endDate = endDate;
            this._projectStatusId = projectStatusId;
            this._productId = productId;
            this._userId = userId;
            this._projectTypeId = projectTypeId;
            this._productManager = pm;
        }

        // Getters - Setters
        [Key]
        public Guid ProjectId { get => this._projectId; private set => this._projectId = Guid.NewGuid(); }
        public string ProjectName { get => this._projectName; set => this._projectName = value; }
        public string? Description { get => this._description; set => this._description = value; }
        public DateTime StartDate { get => this._startDate; set => this._startDate = value; }
        public DateTime EndDate { get => this._endDate; set => this._endDate = value; }
        [Required]
        public Guid ProjectStatusId { get => this._projectStatusId; set => this._projectStatusId = value; }
        [ForeignKey(nameof(ProjectStatusId))]
        public PMIS_ProjectStatus? ProjectStatus { get => this._projectStatus; set => this._projectStatus = value; }
        [Required]
        public Guid ProductId { get => this._productId; set => this._productId = value; }
        [ForeignKey(nameof(ProductId))]
        public PMIS_Product? Product { get => this._product; set => this._product = value; }
        [Required]
        public string UserId { get => this._userId; set => this._userId = value; }
        [ForeignKey(nameof(UserId))]
        public User? User { get => this._user; set => this._user = value; }
        [Required]
        public Guid ProjectTypeId { get => this._projectTypeId; set => this._projectTypeId = value; }
        [ForeignKey(nameof(ProjectTypeId))]
        public PMIS_ProjectType? ProjectType { get => this._projectType; set => this._projectType = value; }
        public string ProductManager { get => this._productManager; set => this._productManager = value; }

        // Relationships
        public List<PMIS_Invite> InviteTokens { get; set; } = new List<PMIS_Invite>();
        public List<PMIS_Task> Tasks { get; set; } = new List<PMIS_Task>();
        public List<PMIS_ProjectTeam> ProjectTeams { get; } = [];
    }
}
