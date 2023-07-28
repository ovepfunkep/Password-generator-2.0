using Microsoft.EntityFrameworkCore;

using PwdGenDLL.Models;

namespace PwdGenDLL.Repositories.Implementations
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

        public void Add(Settings key)
        {
            _dbSet.Add(key);
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                _dbContext.Entry(key).State = EntityState.Detached;
                throw;
            }
        }

        public void Update(Settings key)
        {
            _dbContext.Entry(key).State = EntityState.Modified;
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                _dbContext.Entry(key).State = EntityState.Unchanged;
                throw;
            }
        }

        public void Delete(Settings key)
        {
            try
            {
                _dbContext.Remove(key);
                _dbContext.SaveChanges();
            }
            catch
            {
                _dbContext.Entry(key).State = EntityState.Unchanged;
                throw;
            }
        }

        public void Delete(int id) => Delete(_dbSet.Find(id));
    }
}