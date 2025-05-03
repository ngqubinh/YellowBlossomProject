using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Priority
    {
        private Guid _priorityId;
        private string _priorityName = string.Empty;
        private string _priorityDescription = string.Empty;

        // Constructors 
        public PMIS_Priority() { this._priorityId = Guid.NewGuid(); }
        public PMIS_Priority(string priorityName)
        {
            this._priorityId = Guid.NewGuid();
            this._priorityName = priorityName;
        }
        public PMIS_Priority(string priorityName, string description)
        {
            this._priorityId = Guid.NewGuid();
            this._priorityName = priorityName;
            this._priorityDescription = description;
        }

        // Getters - Setters
        [Key]
        public Guid PriorityId { get => this._priorityId; private set => this._priorityId = Guid.NewGuid(); }
        public string PriorityName { get => this._priorityName; set => this._priorityName = value; }
        public string PriorityDescription { get => this._priorityDescription; set => this._priorityDescription = value; }

        // Relationships
        public List<PMIS_Task> Tasks { get; set; } = new List<PMIS_Task>();
        public List<PMIS_Bug> Bugs { get; set; } = new List<PMIS_Bug>();
    }
}
