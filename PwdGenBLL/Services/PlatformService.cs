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
        
        public IEnumerable<PlatformDTO>? Get(Func<Platform, bool> predicate)
        {
            var entities = _repository.Get(predicate);
            var dto = entities != null ? _converter.ConvertToDTOs(entities) : null;
            return dto;
        }

        public PlatformDTO? Add(PlatformDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Add(entity);
            return _converter.ConvertToDTO(entity);
        }

        public PlatformDTO? Update(PlatformDTO dto)
        {
            var entity = _converter.ConvertToEntity(dto);
            _repository.Update(entity);
            return _converter.ConvertToDTO(entity);
        }

        public void Delete(PlatformDTO dto) => _repository.Delete(_converter.ConvertToEntity(dto));

        public void Delete(int id) => _repository.Delete(id);
    }
}
