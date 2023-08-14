namespace PwdGenBLL.DTOs
{
    public class PasswordHistoryDTO
    {
        public int Id { get; set; }
        public string? SourceText { get; set; }
        public string EncryptedText { get; set; }
        public DateTime? Date { get; set; }
        public SettingsDTO? SettingsDTO { get; set; }

        public PasswordHistoryDTO(string encryptedText, int id = 0, string? sourceText = null, DateTime? date = null, SettingsDTO? settingsDTO = null)
        {
            Id = id;
            SourceText = sourceText;
            EncryptedText = encryptedText;
            Date = date;
            SettingsDTO = settingsDTO;
        }

        public override bool Equals(object? obj)
        {
            return obj is PasswordHistoryDTO dTO &&
                   Id == dTO.Id &&
                   SourceText == dTO.SourceText &&
                   EncryptedText == dTO.EncryptedText &&
                   Date == dTO.Date &&
                   EqualityComparer<SettingsDTO?>.Default.Equals(SettingsDTO, dTO.SettingsDTO);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SourceText, EncryptedText, Date, SettingsDTO);
        }
    }
}
