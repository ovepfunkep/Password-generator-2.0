namespace PwdGenDAL.Models
{
    public class Encryption
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Link { get; set; }
    }
}
