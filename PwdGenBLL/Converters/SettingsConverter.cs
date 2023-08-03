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

        public override SettingsDTO ConvertToDTO(Settings entity)
        {
            var encryptionDTO = _encryptionConverter.ConvertToDTO(entity.Encryption);
            var keyDTO = _keyConverter.ConvertToDTO(entity.Key);

            return new SettingsDTO (encryptionDTO, keyDTO, entity.DateModified, entity.Id);
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
