namespace YellowBlossom.Domain.Models.PMIS
{
    public class PhaseStatus
    {
        private Guid _phaseStatusId;
        private string _phaseStatusName = string.Empty;

        // Constructors
        public PhaseStatus(string phaseStatusName)
        {
            this._phaseStatusId = Guid.NewGuid();
            this._phaseStatusName = phaseStatusName;
        }

        // Getters - Setters
        public Guid ProjectStatusId { get => this._phaseStatusId; private set => this._phaseStatusId = Guid.NewGuid(); }
        public string ProjectStatusName { get => this._phaseStatusName; set => this._phaseStatusName = value; }
    }
}
