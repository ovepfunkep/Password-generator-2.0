using PwdGenBLL.Converters;
using PwdGenBLL.DTOs;

using PwdGenDAL.Models;
using PwdGenDAL.Repositories.Implementations;

namespace PwdGenBLL.Services
{
    public class PlatformService
    {
        private readonly PlatformRepository _repository;
        private readonly PlatformConverter _converter;

        public PlatformService(PlatformRepository repository, PlatformConverter converter)
        {
            _repository = repository;
            _converter = converter;
        }

        public IEnumerable<PlatformDTO> Get()
        {
            var entities = _repository.Get();
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public PlatformDTO? Get(int id)
        {
            var entity = _repository.Get(id);
            var dto = entity != null ? _converter.ConvertToDTO(entity) : null;
            return dto;
        }

        public IEnumerable<PlatformDTO> Get(Func<Platform, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dtos = _converter.ConvertToDTOs(entities);
            return dtos;
        }

        public PlatformDTO? Add(PlatformDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public PlatformDTO Update(PlatformDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            var dbEntity = _repository.Get(entity.Id) ?? throw new Exception("Given entity was not found.");

            dbEntity.Name = dto.Name;
            dbEntity.PasswordHistoryId = dto.PasswordHistoryDTO.Id;
            dbEntity.IconPath = dto.IconPath;
            _repository.Update(dbEntity);

            return _converter.ConvertToDTO(dbEntity);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
