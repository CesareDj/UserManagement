namespace UserManagementAPI.Models
{
    public class BaseEntity
    {
        public string CreatedBy { get; set; } = "ADMIN";
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

}
