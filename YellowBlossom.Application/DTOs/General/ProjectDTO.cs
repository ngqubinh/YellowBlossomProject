namespace YellowBlossom.Application.DTOs.General
{
    public class ProjectDTO
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ProjectStatusId { get; set; }
        public ProjectStatusDTO? ProjectStatusDTO { get; set; }
        public Guid ProductId { get; set; }
        public ProductDTO? ProductDTO { get; set; }
        public string UserId { get; set; } = string.Empty;
        public UserDTO? UserDTO { get; set; }
        public Guid ProjectTypeId { get; set; }
        public ProjectTypeDTO? ProjectTypeDTO { get; set; }
        public string ProductManager { get; set; } = string.Empty;
        public List<ProjectTeamDTO>? ProjectTeam { get; set; }
        public string? Message { get; set; }
    }
}
