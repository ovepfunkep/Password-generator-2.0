using Microsoft.EntityFrameworkCore;

using PwdGenDAL.Models;

namespace PwdGenDAL.Repositories.Implementations
{

    public class PasswordHistoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<PasswordHistory> _dbSet;

        public PasswordHistoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<PasswordHistory>();
        }

        public IEnumerable<PasswordHistory> Get()
        {
            var entities = _dbSet.Include(ph => ph.Settings).ToList();

            foreach (var entity in entities)
            {
                if (entity.Settings != null)
                {
                    _dbContext.Entry(entity.Settings)
                        .Reference(s => s.Encryption)
                        .Load();

                    _dbContext.Entry(entity.Settings)
                        .Reference(s => s.Key)
                        .Load();
                }
            }

            return entities;
        }

        public IEnumerable<PasswordHistory> Get(Func<PasswordHistory, bool> predicate)
        {
            var entities = _dbSet.Include(ph => ph.Settings).Where(predicate).ToList();

            foreach (var entity in entities)
            {
                if (entity.Settings != null)
                {
                    _dbContext.Entry(entity.Settings)
                        .Reference(s => s.Encryption)
                        .Load();

                    _dbContext.Entry(entity.Settings)
                        .Reference(s => s.Key)
                        .Load();
                }
            }

            return entities;
        }


        public PasswordHistory? Get(int id)
        {
            var entity = _dbSet.Include(ph => ph.Settings).FirstOrDefault(ph => ph.Id == id);

            if (entity != null && entity.Settings != null)
            {
                _dbContext.Entry(entity.Settings)
                    .Reference(s => s.Encryption)
                    .Load();

                _dbContext.Entry(entity.Settings)
                    .Reference(s => s.Key)
                    .Load();
            }

            return entity;
        }


        public void Add(PasswordHistory entity)
        {
            _dbSet.Add(entity);
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(_dbSet.Find(entity.Id) ?? throw new("Given entity was not found.")).State = EntityState.Detached; throw; }
        }

        public void Update(PasswordHistory entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            try { _dbContext.SaveChanges(); }
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