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

        public override PasswordHistoryDTO ConvertToDTO(PasswordHistory passwordHistory)
        {
            var settingsDTO = passwordHistory.Settings != null ?
                                        _settingsConverter.ConvertToDTO(passwordHistory.Settings) :
                                        null;

            return new PasswordHistoryDTO
            {
                Id = passwordHistory.Id,
                SourceText = passwordHistory.SourceText,
                EncryptedText = passwordHistory.EncryptedText,
                Date = passwordHistory.Date,
                SettingsDTO = settingsDTO
            };
        }

        public override PasswordHistory ConvertToEntity(PasswordHistoryDTO dto)
        {
            var settingsEntity = dto.SettingsDTO != null ?
                                         _settingsConverter.ConvertToEntity(dto.SettingsDTO) :
                                         null;

            return new PasswordHistory()
            {
                Id = dto.Id,
                SourceText = dto.SourceText,
                EncryptedText = dto.EncryptedText,
                Date = dto.Date,
                SettingsId = dto.SettingsDTO?.Id,
                Settings = settingsEntity
            };
        }
    }
}
