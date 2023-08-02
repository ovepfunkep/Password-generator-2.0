using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

namespace PwdGenBLL.Converters
{
    public class EncryptionConverter : GenericConverter<EncryptionDTO, Encryption>
    {
        public override EncryptionDTO ConvertToDTO(Encryption encryption) =>
            new()
            {
                Id = encryption.Id,
                Name = encryption.Name,
                Description = encryption.Description,
                Link = encryption.Link
            };

        public override Encryption ConvertToEntity(EncryptionDTO dto) =>
            new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Link = dto.Link
            };
    }
}
