using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PwdGenDLL.Models;

namespace PwdGenDLL.Repositories.Implementations
{

    public class EncryptionRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Encryption> _dbSet;

        public EncryptionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<Encryption>();
        }

        public IEnumerable<Encryption> Get() => _dbSet.ToList();

        public IEnumerable<Encryption> Get(Func<Encryption, bool> predicate) => _dbSet.Where(predicate).ToList();

        public Encryption? Get(int id) => _dbSet.Find(id);

        public void Add(Encryption key)
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

        public void Update(Encryption key)
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

        public void Delete(Encryption key)
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