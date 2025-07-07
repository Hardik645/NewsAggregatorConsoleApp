namespace NewsAggregatorConsoleApp.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
    }
}
