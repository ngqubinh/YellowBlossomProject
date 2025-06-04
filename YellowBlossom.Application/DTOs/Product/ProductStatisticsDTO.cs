using YellowBlossom.Application.DTOs.General;

namespace YellowBlossom.Application.DTOs.Product
{
    public class ProductStatisticsDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public int TotalProjects { get; set; }
        public Dictionary<string, int> ProjectStatusDistribution { get; set; } = new Dictionary<string, int>();
        public List<ProjectDTO> Projects { get; set; } = new List<ProjectDTO>();
    }

    public class ProductSummaryDTO
    {

    }
}
