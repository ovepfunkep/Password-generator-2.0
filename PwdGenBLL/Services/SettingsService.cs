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
        public IEnumerable<SettingsDTO>? Get(Func<Settings, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dto = entities != null ? _converter.ConvertToDTOs(entities) : null;
            return dto;
        }

        public SettingsDTO? Add(SettingsDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public SettingsDTO? Update(SettingsDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Update(entity);
            return _converter.ConvertToDTO(entity);
        }

        public void Delete(SettingsDTO dto) => _repository.Delete(_converter.ConvertToEntity(dto));

        public void Delete(int id) => _repository.Delete(id);
    }
}
