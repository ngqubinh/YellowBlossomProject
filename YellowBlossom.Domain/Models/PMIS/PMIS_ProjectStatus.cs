using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_ProjectStatus
    {
        private Guid _projectStatusId;
        private string _projectStatusName = string.Empty;

        // Constructors
        public PMIS_ProjectStatus(string projectStatusName)
        {
            this._projectStatusId = Guid.NewGuid();
            this._projectStatusName = projectStatusName;
        }

        // Getters - Setters
        [Key]
        public Guid ProjectStatusId { get => this._projectStatusId; private set => this._projectStatusId = Guid.NewGuid(); }
        public string ProjectStatusName { get => this._projectStatusName; set => this._projectStatusName = value; }

        // Relationship
        public List<PMIS_Project> Projects { get; set; } = new List<PMIS_Project>();
    }
}
