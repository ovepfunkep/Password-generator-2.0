using PwdGenDAL.Models;

using PwdGenBLL.DTOs;

namespace PwdGenBLL.Converters
{
    public class PasswordHistoryConverter : GenericConverter<PasswordHistoryDTO, PasswordHistory>
    {
        private readonly SettingsConverter _settingsConverter;

        public PasswordHistoryConverter(SettingsConverter settingsConverter)
        {
            _settingsConverter = settingsConverter;
        }

        public override PasswordHistoryDTO ConvertToDTO(PasswordHistory entity)
        {
            var settingsDTO = entity.Settings != null ?
                                        _settingsConverter.ConvertToDTO(entity.Settings) :
                                        null;

            return new PasswordHistoryDTO(entity.EncryptedText, entity.Id, entity.SourceText, entity.Date, settingsDTO);
        }

        public override PasswordHistory ConvertToEntity(PasswordHistoryDTO entityDTO)
        {
            var settingsEntity = entityDTO.SettingsDTO != null ?
                                         _settingsConverter.ConvertToEntity(entityDTO.SettingsDTO) :
                                         null;

            return new PasswordHistory()
            {
                Id = entityDTO.Id,
                SourceText = entityDTO.SourceText,
                EncryptedText = entityDTO.EncryptedText,
                Date = entityDTO.Date,
                SettingsId = entityDTO.SettingsDTO?.Id,
                Settings = settingsEntity
            };
        }
    }
}
