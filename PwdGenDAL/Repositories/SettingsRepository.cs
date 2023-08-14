using Microsoft.EntityFrameworkCore;

using PwdGenDAL.Models;

namespace PwdGenDAL.Repositories.Implementations
{

    public class SettingsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Settings> _dbSet;

        public SettingsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<Settings>();
        }

        public IEnumerable<Settings> Get() => _dbSet.Include(s => s.Encryption)
                                                    .Include(s => s.Key).ToList();

        public IEnumerable<Settings> Get(Func<Settings, bool> predicate) => _dbSet.Include(s => s.Encryption)
                                                                                  .Include(s => s.Key)
                                                                                  .Where(predicate).ToList();

        public Settings? Get(int id) => _dbSet.Include(s => s.Encryption)
                                              .Include(s => s.Key)
                                              .FirstOrDefault(s => s.Id == id);

        public void Add(Settings entity)
        {
            _dbSet.Add(entity);
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Detached; throw; }
        }

        public void Update(Settings entity)
        {
            _dbContext.Entry(_dbSet.Find(entity.Id) ?? throw new("Given entity was not found.")).State = EntityState.Modified;
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