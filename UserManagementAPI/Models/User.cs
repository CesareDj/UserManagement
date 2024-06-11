namespace UserManagementAPI.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string First { get; set; } = string.Empty;
        public string Last { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country? Country { get; set; } 
        public int CompanyId { get; set; }
        public Company? Company { get; set; } 
    }
}
