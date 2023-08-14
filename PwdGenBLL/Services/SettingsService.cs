using PwdGenBLL.Converters;
using PwdGenBLL.DTOs;

using PwdGenDAL.Models;
using PwdGenDAL.Repositories.Implementations;

namespace PwdGenBLL.Services
{
    public class SettingsService
    {
        private readonly SettingsRepository _repository;
        private readonly SettingsConverter _converter;

        public SettingsService(SettingsRepository repository, SettingsConverter converter)
        {
            _repository = repository;
            _converter = converter;
        }

        public IEnumerable<SettingsDTO> Get()
        {
            var entities = _repository.Get();
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public SettingsDTO? Get(int id)
        {
            var entity = _repository.Get(id);
            var dto = entity != null ? _converter.ConvertToDTO(entity) : null;
            return dto;
        }

        public IEnumerable<SettingsDTO> Get(Func<Settings, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public SettingsDTO? Add(SettingsDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public SettingsDTO Update(SettingsDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            var dbEntity = _repository.Get(entity.Id) ?? throw new Exception("Given entity was not found.");

            dbEntity.DateModified = dto.DateModified;
            dbEntity.EncryptionId = dto.EncryptionDTO.Id;
            dbEntity.KeyId = dto.EncryptionDTO.Id;

            _repository.Update(dbEntity);

            return _converter.ConvertToDTO(dbEntity);
        }


        public void Delete(int id) => _repository.Delete(id);
    }
}
