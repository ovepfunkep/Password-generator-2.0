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

        public IEnumerable<Key> Get(Func<Key, bool> predicate) => _dbSet.Where(predicate);

        public Key? Get(int id) => _dbSet.Find(id);

        public void Add(Key entity)
        {
            _dbSet.Add(entity);
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Detached; throw; }
        }

        public void Update(Key entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Unchanged; throw; }
        }

        public void Delete(Key entity)
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