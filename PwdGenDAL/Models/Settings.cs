namespace PwdGenDAL.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public DateTime? DateModified { get; set; }
        public int EncryptionId { get; set; }
        public int KeyId { get; set; }

        // Relationships
        public Encryption Encryption { get; set; } = null!;
        public Key Key { get; set; } = null!;
    }
}
