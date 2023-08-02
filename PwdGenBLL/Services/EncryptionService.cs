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
    public class EncryptionService
    {
        private readonly EncryptionRepository _repository;
        private readonly EncryptionConverter _converter;

        public EncryptionService(EncryptionRepository repository, EncryptionConverter converter)
        {
            _repository = repository;
            _converter = converter;
        }

        public IEnumerable<EncryptionDTO> Get()
        {
            var entities = _repository.Get();
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public EncryptionDTO? Get(int id)
        {
            var entity = _repository.Get(id);
            var dto = entity != null ? _converter.ConvertToDTO(entity) : null;
            return dto;
        }
        public IEnumerable<EncryptionDTO>? Get(Func<Encryption, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dto = entities != null ? _converter.ConvertToDTOs(entities) : null;
            return dto;
        }

        public EncryptionDTO? Add(EncryptionDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public EncryptionDTO? Update(EncryptionDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Update(entity);
            return _converter.ConvertToDTO(entity);
        }

        public void Delete(EncryptionDTO dto) => _repository.Delete(_converter.ConvertToEntity(dto));

        public void Delete(int id) => _repository.Delete(id);
    }
}
