namespace PwdGenDAL.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int PasswordHistoryId { get; set; }
        public string? IconPath { get; set; }

        // Relationships
        public PasswordHistory PasswordHistory { get; set; } = null!;
    }
}
