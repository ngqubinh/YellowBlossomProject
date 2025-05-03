namespace YellowBlossom.Application.DTOs.Team
{
    public class UpdateUserLockRequest
    {
        public string UserId { get; set; } = string.Empty;
        public bool Lock { get; set; }
        public int LockDurationDays { get; set; }
    }
}
