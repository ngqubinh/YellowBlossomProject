namespace YellowBlossom.Application.DTOs.General
{
    public class TaskDTO
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid PriorityId { get; set; }
        public Guid TastStatusId { get; set; }
        public Guid ProjectId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public Guid? AssignedTo {  get; set; }
        public PriorityDTO? Priority { get; set; }
        public TaskStatusDTO? TaskStatus { get; set; }
        public ProjectDTO? Project { get; set; }
        public UserDTO? User { get; set; }
        public TeamDTO? Team { get; set; }
        public string? Message { get; set; }
    }

    public class TaskTestDTO
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid PriorityId { get; set; }
        public Guid TastStatusId { get; set; }
        public Guid ProjectId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public Guid? AssignedTo { get; set; }
        public PriorityDTO? Priority { get; set; }
        public TaskStatusDTO? TaskStatus { get; set; }
        public ProjectDTO? Project { get; set; }
        public UserDTO? User { get; set; }
        public TeamDTO? Team { get; set; }
        public string? Message { get; set; }
        public List<TestCaseDTO>? TestCase { get; set; }
        public List<TestRunDTO>? TestRunDTO { get; set; }
        public List<TestRunTestCaseDTO>? TestRunTestCases { get; set; }
    }
}
