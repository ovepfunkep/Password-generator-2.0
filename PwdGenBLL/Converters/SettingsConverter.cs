using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

namespace PwdGenBLL.Converters
{
    public class SettingsConverter : GenericConverter<SettingsDTO, Settings>
    {
        private readonly GenericConverter<EncryptionDTO, Encryption> _encryptionConverter;
        private readonly GenericConverter<KeyDTO, Key> _keyConverter;

        public SettingsConverter(GenericConverter<EncryptionDTO, Encryption> encryptionConverter,
                                 GenericConverter<KeyDTO, Key> keyConverter)
        {
            _encryptionConverter = encryptionConverter;
            _keyConverter = keyConverter;
        }

        public override SettingsDTO ConvertToDTO(Settings settings)
        {
            var encryptionDTO = _encryptionConverter.ConvertToDTO(settings.Encryption);
            var keyDTO = _keyConverter.ConvertToDTO(settings.Key);

            return new SettingsDTO 
            {
                Id = settings.Id,
                DateModified = settings.DateModified,
                EncryptionDTO = encryptionDTO,
                KeyDTO = keyDTO
            };
        }

        public override Settings ConvertToEntity(SettingsDTO dto) => new()
            {
                Id = dto.Id,
                DateModified = dto.DateModified,
                EncryptionId = dto.EncryptionDTO.Id,
                KeyId = dto.KeyDTO.Id
            };
    }
}
