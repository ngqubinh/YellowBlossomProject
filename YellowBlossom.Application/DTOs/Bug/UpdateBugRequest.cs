namespace YellowBlossom.Application.DTOs.Bug
{
    public class UpdateBugRequest
    {
        public string StepsToReduce { get; set; } = string.Empty;
        public string Serverity {  get; set; } = string.Empty;
        public Guid AssignedToTeamId { get; set; }
    }
}
