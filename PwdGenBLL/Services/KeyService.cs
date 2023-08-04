using PwdGenBLL.Converters;

using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenBLL.Services
{
    public class KeyService
    {
        private readonly KeyRepository _repository;
        private readonly KeyConverter _converter;

        public KeyService(KeyRepository repository, KeyConverter converter)
        {
            _repository = repository;
            _converter = converter;
        }

        public IEnumerable<KeyDTO> Get()
        {
            var entities = _repository.Get();
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public KeyDTO? Get(int id)
        {
            var entity = _repository.Get(id);
            var dto = entity != null ? _converter.ConvertToDTO(entity) : null;
            return dto;
        }

        public IEnumerable<KeyDTO> Get(Func<Key, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public KeyDTO? Add(KeyDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public KeyDTO Update(KeyDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            var dbEntity = _repository.Get(entity.Id) ?? throw new Exception("Given entity was not found.");

            dbEntity.Value = dto.Value;
            _repository.Update(dbEntity);

            return _converter.ConvertToDTO(dbEntity);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
