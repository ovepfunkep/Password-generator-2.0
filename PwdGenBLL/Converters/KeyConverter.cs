using PwdGenDAL.Models;

using PwdGenBLL.DTOs;

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
