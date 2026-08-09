namespace UserManagementAPI.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string First { get; set; } = string.Empty;
        public string Last { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
