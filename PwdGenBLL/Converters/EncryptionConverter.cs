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
        public override EncryptionDTO ConvertToDTO(Encryption entity) => new(entity.Name, entity.Id, entity.Description, entity.Link);

        public override Encryption ConvertToEntity(EncryptionDTO entityDTO) =>
            new()
            {
                Id = entityDTO.Id,
                Name = entityDTO.Name,
                Description = entityDTO.Description,
                Link = entityDTO.Link
            };
    }
}
