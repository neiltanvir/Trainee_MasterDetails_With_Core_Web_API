namespace TraineeCoreAPI.DTOs
{
    public class UserModel
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? UserMessage { get; set; }
        public string? AccessToken { get; set; }
    }
}
