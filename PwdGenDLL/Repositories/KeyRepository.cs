using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PwdGenDLL.Models;

namespace PwdGenDLL.Repositories.Implementations
{

    public class KeyRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Key> _dbSet;

        public KeyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<Key>();
        }

        public IEnumerable<Key> Get() => _dbSet.ToList();

        public IEnumerable<Key> Get(Func<Key, bool> predicate) => _dbSet.Where(predicate).ToList();

        public Key? Get(int id) => _dbSet.Find(id);

        public void Add(Key key)
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

        public void Update(Key key)
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

        public void Delete(Key key)
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