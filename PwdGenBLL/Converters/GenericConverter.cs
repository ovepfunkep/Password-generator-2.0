using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdGenBLL.Converters
{
    public abstract class GenericConverter<TDTO, T>
    {
        public abstract TDTO ConvertToDTO(T entity);
        public abstract T ConvertToEntity(TDTO dto);

        public IEnumerable<TDTO> ConvertToDTOs(IEnumerable<T> entities) => entities.Select(entity => ConvertToDTO(entity));

        public IEnumerable<T> ConvertToEntities(IEnumerable<TDTO> dtos) => dtos.Select(dto => ConvertToEntity(dto));
    }
}
