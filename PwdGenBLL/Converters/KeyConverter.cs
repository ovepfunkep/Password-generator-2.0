using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

namespace PwdGenBLL.Converters
{
    public class KeyConverter : GenericConverter<KeyDTO, Key>
    {
        public override KeyDTO ConvertToDTO(Key entity) => new(entity.Value, entity.Id);

        public override Key ConvertToEntity(KeyDTO entityDTO) => new() 
        { 
            Id = entityDTO.Id,
            Value = entityDTO.Value
        };
    }
}
