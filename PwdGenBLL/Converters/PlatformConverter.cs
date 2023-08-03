using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

namespace PwdGenBLL.Converters
{
    public class PlatformConverter : GenericConverter<PlatformDTO, Platform>
    {
        private readonly GenericConverter<PasswordHistoryDTO, PasswordHistory> _passwordHistoryConverter;

        public PlatformConverter(GenericConverter<PasswordHistoryDTO, PasswordHistory> passwordHistoryConverter)
        {
            _passwordHistoryConverter = passwordHistoryConverter;
        }

        public override PlatformDTO ConvertToDTO(Platform entity)
        {
            var passwordHistoryDTO = _passwordHistoryConverter.ConvertToDTO(entity.PasswordHistory);

            return new PlatformDTO(entity.Name, passwordHistoryDTO, entity.Id, entity.IconPath);
        }

        public override Platform ConvertToEntity(PlatformDTO entityDTO)
        {
            var passwordHistoryEntity = _passwordHistoryConverter.ConvertToEntity(entityDTO.PasswordHistoryDTO);

            return new Platform
            {
                Id = entityDTO.Id,
                Name = entityDTO.Name,
                PasswordHistory = passwordHistoryEntity,
                IconPath = entityDTO.IconPath                                          
            };
        }
    }
}
