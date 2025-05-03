using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Phase
    {
        private Guid _phaseId;
        private string _phaseName = string.Empty;
        private DateTime _startDate;
        private DateTime _endDate;
        private Guid _phaseStatusId;
        private PMIS_PhaseStatus? _phaseStatus;
        private Guid _projectId;
        private PMIS_Project? _project;

        // Constructors
        public PMIS_Phase() { this._phaseId = Guid.NewGuid(); }
        public PMIS_Phase(string phaseName, DateTime startDate, DateTime endDate, Guid phaseStatusId, Guid projectId)
        {
            this._phaseId = Guid.NewGuid();
            this._phaseName = phaseName;
            this._startDate = startDate;
            this._endDate = endDate;
            this._phaseStatusId = phaseStatusId;
            this._projectId = projectId;
        }

        // Getters - Setters
        [Key]
        public Guid PhaseId { get => this._phaseId; private set => this._phaseId = Guid.NewGuid(); }
        public string PhaseName { get => this._phaseName; set => this._phaseName = value; }
        public DateTime StartDate { get => this._startDate; set => this._startDate = value; }
        public DateTime EndDate { get => this._endDate; set => this._endDate = value; }

        // Relationships
        [Required]
        public Guid PhaseStatusId { get => this._phaseStatusId; set => this._phaseStatusId = value; }
        [ForeignKey(nameof(PhaseId))]
        public PMIS_PhaseStatus? PhaseStatus { get => this._phaseStatus; set => this._phaseStatus = value; }
        [Required]
        public Guid ProjectId { get => this._projectId; set => this._projectId = value; }
        [ForeignKey(nameof(ProjectId))]
        public PMIS_Project? Project { get => this._project; set => this._project = value; }
    }
}
