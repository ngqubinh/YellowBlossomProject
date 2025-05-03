using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Application.DTOs.General
{
    public class PriorityDTO
    {
        public Guid PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public string PriorityDescription { get; set; } = string.Empty;

        // Relationships
        public List<PMIS_Bug> Bugs { get; set; } = new List<PMIS_Bug>();
    }
}
