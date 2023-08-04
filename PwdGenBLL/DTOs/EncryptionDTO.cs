namespace PwdGenDLL.Models
{
    public class EncryptionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }

        public EncryptionDTO(string name, int id = 0, string? description = null, string? link = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Link = link;
        }

        public override bool Equals(object? obj)
        {
            return obj is EncryptionDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Description == dTO.Description &&
                   Link == dTO.Link;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, Link);
        }
    }
}
