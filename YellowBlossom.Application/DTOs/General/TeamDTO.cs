namespace YellowBlossom.Application.DTOs.General
{
    public class TeamDTO
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string? TeamDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<UserDTO>? Users { get; set; }
        public string? Message { get; set; }
        public TeamDTO() { }
        public TeamDTO(string message) { this.Message = message; }
    }
}
