using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TaskStatus
    {
        private Guid _taskStatusId;
        private string _taskStatusName = string.Empty;

        // Constructors
        public PMIS_TaskStatus() { this._taskStatusId = Guid.NewGuid(); }
        public PMIS_TaskStatus(string taskStatusName)
        {
            this._taskStatusName = taskStatusName;
        }

        // Getters - Setters
        [Key]
        public Guid TaskStatusId { get => this._taskStatusId; private set => this._taskStatusId = Guid.NewGuid(); }
        public string TaskStatusName { get => this._taskStatusName; set => this._taskStatusName = value; }

        // Relationships
        public List<PMIS_Task> Tasks { get; set; } = new List<PMIS_Task>();
    }
}
