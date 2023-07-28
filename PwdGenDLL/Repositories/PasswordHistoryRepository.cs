using Microsoft.EntityFrameworkCore;

using PwdGenDLL.Models;

namespace PwdGenDLL.Repositories.Implementations
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
            var passwordHistories = _dbSet.Include(ph => ph.Settings).ToList();

            foreach (var passwordHistory in passwordHistories)
            {
                if (passwordHistory.Settings != null)
                {
                    _dbContext.Entry(passwordHistory.Settings)
                        .Reference(s => s.Encryption)
                        .Load();

                    _dbContext.Entry(passwordHistory.Settings)
                        .Reference(s => s.Key)
                        .Load();
                }
            }

            return passwordHistories;
        }

        public IEnumerable<PasswordHistory> Get(Func<PasswordHistory, bool> predicate)
        {
            var passwordHistories = _dbSet.Include(ph => ph.Settings).Where(predicate).ToList();

            foreach (var passwordHistory in passwordHistories)
            {
                if (passwordHistory.Settings != null)
                {
                    _dbContext.Entry(passwordHistory.Settings)
                        .Reference(s => s.Encryption)
                        .Load();

                    _dbContext.Entry(passwordHistory.Settings)
                        .Reference(s => s.Key)
                        .Load();
                }
            }

            return passwordHistories;
        }


        public PasswordHistory? Get(int id)
        {
            var passwordHistory = _dbSet.Include(ph => ph.Settings).FirstOrDefault(ph => ph.Id == id);

            if (passwordHistory != null && passwordHistory.Settings != null)
            {
                _dbContext.Entry(passwordHistory.Settings)
                    .Reference(s => s.Encryption)
                    .Load();

                _dbContext.Entry(passwordHistory.Settings)
                    .Reference(s => s.Key)
                    .Load();
            }

            return passwordHistory;
        }


        public void Add(PasswordHistory key)
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

        public void Update(PasswordHistory key)
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

        public void Delete(PasswordHistory key)
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