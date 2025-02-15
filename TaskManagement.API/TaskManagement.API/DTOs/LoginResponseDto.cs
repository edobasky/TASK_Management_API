namespace TaskManagement.API.DTOs
{
    // public record LoginResponseDto(int userId, string userName, string email, string Token, int tokenExpireIn);


    public class LoginResponseDto
    {
        public int userId { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? Token { get; set; }
        public int tokenExpireIn { get; set; }
    }
}


