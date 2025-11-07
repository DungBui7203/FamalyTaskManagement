namespace API.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public long UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public long FamilyId { get; set; }
    }
}
