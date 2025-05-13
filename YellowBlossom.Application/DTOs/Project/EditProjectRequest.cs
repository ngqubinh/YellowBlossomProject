namespace YellowBlossom.Application.DTOs.Project
{
    public class EditProjectRequest : CreateProjectRequest
    {
        public Guid ProjectStatusId { get; set; }
        public Guid ProjectTypeId { get; set; }
    }

    public class EditProjectStatusRequest
    {
        public Guid ProjectStatusId { get; set; }
    }
}
