using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_ProjectType
    {
        private Guid _projectTypeId;
        private string _projectTypeName = string.Empty;

        // Constructors
        public PMIS_ProjectType(string projectTypeName)
        {
            this._projectTypeId = Guid.NewGuid();
            this._projectTypeName = projectTypeName;
        }

        // Getters - Setters
        [Key]
        public Guid ProjectTypeId { get => this._projectTypeId; private set => this._projectTypeId = Guid.NewGuid(); }
        public string ProjectTypeName { get => this._projectTypeName; set => this._projectTypeName = value; }

        // Relationship
        public List<PMIS_Project> Projects { get; set; } = new List<PMIS_Project>();
    }
}
