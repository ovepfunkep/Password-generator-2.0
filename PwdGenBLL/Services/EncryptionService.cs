using PwdGenBLL.Converters;
using PwdGenBLL.DTOs;

using PwdGenDAL.Models;
using PwdGenDAL.Repositories.Implementations;

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

        public IEnumerable<EncryptionDTO> Get(Func<Encryption, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public EncryptionDTO? Add(EncryptionDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public EncryptionDTO Update(EncryptionDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            var dbEntity = _repository.Get(entity.Id) ?? throw new Exception("Given entity was not found.");

            dbEntity.Name = dto.Name;
            dbEntity.Description = dto.Description;
            dbEntity.Link = dto.Link;
            _repository.Update(dbEntity);

            return _converter.ConvertToDTO(dbEntity);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
