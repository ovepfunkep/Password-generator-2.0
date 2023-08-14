namespace PwdGenBLL.DTOs
{
    public class SettingsDTO
    {
        public int Id { get; set; }
        public DateTime? DateModified { get; set; }
        public EncryptionDTO EncryptionDTO { get; set; }
        public KeyDTO KeyDTO { get; set; }

        public SettingsDTO(EncryptionDTO encryptionDTO, KeyDTO keyDTO, DateTime? dateModified = null, int id = 0)
        {
            Id = id;
            DateModified = dateModified;
            EncryptionDTO = encryptionDTO;
            KeyDTO = keyDTO;
        }

        public override bool Equals(object? obj)
        {
            return obj is SettingsDTO dTO &&
                   Id == dTO.Id &&
                   DateModified == dTO.DateModified &&
                   EqualityComparer<EncryptionDTO>.Default.Equals(EncryptionDTO, dTO.EncryptionDTO) &&
                   EqualityComparer<KeyDTO>.Default.Equals(KeyDTO, dTO.KeyDTO);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, DateModified, EncryptionDTO, KeyDTO);
        }
    }
}
