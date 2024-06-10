namespace UserManagementAPI.Models
{
    public class Country : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = [];
    }
}
