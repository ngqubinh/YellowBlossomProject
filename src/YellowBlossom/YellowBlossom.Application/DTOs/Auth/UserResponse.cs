namespace YellowBlossom.Application.DTOs.Auth
{
    public class UserResponse
    {
        public string? AccessToken { get; set; }
        public string? Message { get; set; }

        // Constructors
        public UserResponse() { }
        public UserResponse(string message)
        {
            this.Message = message;
        }
    }
}
