using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Application.DTOs.General
{
    public class PhaseDTO
    {
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid PhaseStatusId { get; set; }
        public PhaseStatusDTO? PhaseStatusDTO { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectDTO? ProjectDTO { get; set; }
    }
}
