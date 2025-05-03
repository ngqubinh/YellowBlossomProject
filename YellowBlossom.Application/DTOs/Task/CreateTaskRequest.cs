namespace YellowBlossom.Application.DTOs.Task
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid PriorityId { get; set; }
        public Guid TaskStatusId { get; set; }
        public Guid ProjectId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public Guid? AssignedTo { get; set; }
    }
}
