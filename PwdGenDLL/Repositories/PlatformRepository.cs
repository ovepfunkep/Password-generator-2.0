using Microsoft.EntityFrameworkCore;

using PwdGenDLL.Models;

namespace PwdGenDLL.Repositories.Implementations
{

    public class PlatformRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Platform> _dbSet;

        public PlatformRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<Platform>();
        }

        public IEnumerable<Platform> Get()
        {
            var entities = _dbSet.Include(s => s.PasswordHistory)
                                                 .ThenInclude(ph => ph.Settings)
                                             .ToList();

            foreach (var entity in entities)
            {
                if (entity.PasswordHistory.Settings != null)
                {
                    _dbContext.Entry(entity.PasswordHistory.Settings)
                        .Reference(s => s.Encryption)
                        .Load();

                    _dbContext.Entry(entity.PasswordHistory.Settings)
                        .Reference(s => s.Key)
                        .Load();
                }
            }

            return entities;
        }

        public IEnumerable<Platform> Get(Func<Platform, bool> predicate)
        {
            var entities = _dbSet.Include(s => s.PasswordHistory)
                                                 .ThenInclude(ph => ph.Settings)
                                            .Where(predicate)
                                             .ToList();

            foreach (var entity in entities)
            {
                if (entity.PasswordHistory.Settings != null)
                {
                    _dbContext.Entry(entity.PasswordHistory.Settings)
                        .Reference(s => s.Encryption)
                        .Load();

                    _dbContext.Entry(entity.PasswordHistory.Settings)
                        .Reference(s => s.Key)
                        .Load();
                }
            }

            return entities;
        }


        public Platform? Get(int id)
        {
            var entity = _dbSet.Include(s => s.PasswordHistory)
                                          .ThenInclude(ph => ph.Settings)
                                      .FirstOrDefault(s => s.Id == id);

            if (entity != null && entity.PasswordHistory.Settings != null)
            {
                _dbContext.Entry(entity.PasswordHistory.Settings)
                    .Reference(s => s.Encryption)
                    .Load();

                _dbContext.Entry(entity.PasswordHistory.Settings)
                    .Reference(s => s.Key)
                    .Load();
            }

            return entity;
        }


        public void Add(Platform entity)
        {
            _dbSet.Add(entity);
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Detached; throw; }
        }

        public void Update(Platform entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Unchanged; throw; }
        }

        public void Delete(Platform entity)
        {
            try
            {
                _dbContext.Remove(entity);
                _dbContext.SaveChanges();
            }
            catch { _dbContext.Entry(entity).State = EntityState.Unchanged; throw; }
        }

        public void Delete(int id)
        {
            try
            {
                _dbContext.Remove(_dbSet.Find(id) ?? throw new("Given entity was not found."));
                _dbContext.SaveChanges();
            }
            catch { _dbContext.Entry(id).State = EntityState.Unchanged; throw; }
        }
    }
}