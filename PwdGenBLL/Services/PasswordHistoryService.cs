using PwdGenBLL.Converters;
using PwdGenBLL.DTOs;

using PwdGenDAL.Models;
using PwdGenDAL.Repositories.Implementations;

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

        public IEnumerable<PasswordHistoryDTO> Get(Func<PasswordHistory, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public PasswordHistoryDTO? Add(PasswordHistoryDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public PasswordHistoryDTO Update(PasswordHistoryDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            var dbEntity = _repository.Get(entity.Id) ?? throw new Exception("Given entity was not found.");

            dbEntity.EncryptedText = dto.EncryptedText;
            dbEntity.SourceText = dto.SourceText;
            dbEntity.Date = dto.Date;
            dbEntity.SettingsId = dto.SettingsDTO?.Id;
            _repository.Update(dbEntity);

            return _converter.ConvertToDTO(dbEntity);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
