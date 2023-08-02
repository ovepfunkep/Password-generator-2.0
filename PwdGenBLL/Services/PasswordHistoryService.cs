using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenBLL.Converters;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenBLL.Services
{
    public class PasswordHistoryService
    {
        private readonly PasswordHistoryRepository _repository;
        private readonly PasswordHistoryConverter _converter;

        public PasswordHistoryService(PasswordHistoryRepository repository, PasswordHistoryConverter converter)
        {
            _repository = repository;
            _converter = converter;
        }

        public IEnumerable<PasswordHistoryDTO> Get()
        {
            var entities = _repository.Get();
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public PasswordHistoryDTO? Get(int id)
        {
            var entity = _repository.Get(id);
            var dto = entity != null ? _converter.ConvertToDTO(entity) : null;
            return dto;
        }
        
        public IEnumerable<PasswordHistoryDTO>? Get(Func<PasswordHistory, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dto = entities != null ? _converter.ConvertToDTOs(entities) : null;
            return dto;
        }

        public PasswordHistoryDTO? Add(PasswordHistoryDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public PasswordHistoryDTO? Update(PasswordHistoryDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Update(entity);
            return _converter.ConvertToDTO(entity);
        }

        public void Delete(PasswordHistoryDTO dto) => _repository.Delete(_converter.ConvertToEntity(dto));

        public void Delete(int id) => _repository.Delete(id);
    }
}
