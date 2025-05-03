using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_PhaseStatus
    {
        private Guid _phaseStatusId;
        private string _phaseStatusName = string.Empty;

        // Constructors
        public PMIS_PhaseStatus() { this._phaseStatusId = Guid.NewGuid(); }
        public PMIS_PhaseStatus(string phaseStatusName)
        {
            this._phaseStatusId = Guid.NewGuid();
            this._phaseStatusName = phaseStatusName;
        }

        // Getters - Setters
        [Key]
        public Guid PhaseStatusId { get => this._phaseStatusId; private set => this._phaseStatusId = Guid.NewGuid(); }
        public string PhaseStatusName { get => this._phaseStatusName; set => this._phaseStatusName = value; }

        // Relationships
        public List<PMIS_Phase> Phases { get; set; } = new List<PMIS_Phase>();
    }
}
