using PwdGenDLL.Models;

namespace PwdGenBLL.Converters
{
    public class SettingsConverter : GenericConverter<SettingsDTO, Settings>
    {
        private readonly EncryptionConverter _encryptionConverter;
        private readonly KeyConverter _keyConverter;

        public SettingsConverter(EncryptionConverter encryptionConverter,
                                 KeyConverter keyConverter)
        {
            _encryptionConverter = encryptionConverter;
            _keyConverter = keyConverter;
        }

        public override SettingsDTO ConvertToDTO(Settings entity)
        {
            var encryptionDTO = _encryptionConverter.ConvertToDTO(entity.Encryption);
            var keyDTO = _keyConverter.ConvertToDTO(entity.Key);

            return new SettingsDTO(encryptionDTO, keyDTO, entity.DateModified, entity.Id);
        }

        public override Settings ConvertToEntity(SettingsDTO entity)
        {
            var encryption = _encryptionConverter.ConvertToEntity(entity.EncryptionDTO);
            var key = _keyConverter.ConvertToEntity(entity.KeyDTO);

            return new Settings
            {
                Id = entity.Id,
                Encryption = encryption,
                Key = key,
                DateModified = entity.DateModified
            };
        }
    }
}
