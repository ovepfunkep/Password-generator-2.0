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
        public override KeyDTO ConvertToDTO(Key key) => new() 
        { 
            Id = key.Id, 
            Value = key.Value 
        };

        public override Key ConvertToEntity(KeyDTO dto) => new() 
        { 
            Id = dto.Id,
            Value = dto.Value
        };
    }
}
