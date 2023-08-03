using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

namespace PwdGenBLL.Converters
{
    public class PasswordHistoryConverter : GenericConverter<PasswordHistoryDTO, PasswordHistory>
    {
        private readonly GenericConverter<SettingsDTO, Settings> _settingsConverter;

        public PasswordHistoryConverter(GenericConverter<SettingsDTO, Settings> settingsConverter)
        {
            _settingsConverter = settingsConverter;
        }

        public override PasswordHistoryDTO ConvertToDTO(PasswordHistory entity)
        {
            var settingsDTO = entity.Settings != null ?
                                        _settingsConverter.ConvertToDTO(entity.Settings) :
                                        null;

            return new PasswordHistoryDTO (entity.EncryptedText, entity.Id, entity.SourceText, entity.Date, settingsDTO);
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
