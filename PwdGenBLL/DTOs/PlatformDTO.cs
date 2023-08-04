namespace PwdGenDLL.Models
{
    public class PlatformDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PasswordHistoryDTO PasswordHistoryDTO { get; set; }
        public string? IconPath { get; set; }

        public PlatformDTO(string name, PasswordHistoryDTO passwordHistoryDTO, int id = 0, string? iconPath = null)
        {
            Id = id;
            Name = name;
            PasswordHistoryDTO = passwordHistoryDTO;
            IconPath = iconPath;
        }

        public override bool Equals(object? obj)
        {
            return obj is PlatformDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   EqualityComparer<PasswordHistoryDTO>.Default.Equals(PasswordHistoryDTO, dTO.PasswordHistoryDTO) &&
                   IconPath == dTO.IconPath;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, PasswordHistoryDTO, IconPath);
        }
    }
}
