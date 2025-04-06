namespace YellowBlossom.Domain.Models.PMIS
{
    public class ProjectStatus
    {
        private Guid _projectStatusId;
        private string _projectStatusName = string.Empty;

        // Constructors
        public ProjectStatus(string projectStatusName)
        {
            this._projectStatusId = Guid.NewGuid();
            this._projectStatusName = projectStatusName;
        }

        // Getters - Setters
        public Guid ProjectStatusId { get => this._projectStatusId; private set => this._projectStatusId = Guid.NewGuid(); }
        public string ProjectStatusName { get => this._projectStatusName; set => this._projectStatusName = value; }

        // Relationship
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
