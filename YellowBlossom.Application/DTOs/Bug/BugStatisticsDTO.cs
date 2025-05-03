namespace YellowBlossom.Application.DTOs.Bug
{
    public class BugStatusCountDTO
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class BugSeverityCountDTO
    {
        public string Severity { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class BugPriorityCountDTO
    {
        public string Priority { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class BugTeamCountDTO
    {
        public string TeamName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class BugStatisticsDTO
    {
        public List<BugStatusCountDTO> BugStatusStats { get; set; } = new();
        public List<BugSeverityCountDTO> SeverityStats { get; set; } = new();
        public List<BugPriorityCountDTO> PriorityStats { get; set; } = new();
        public List<BugTeamCountDTO> TeamStats { get; set; } = new();
    }

}
