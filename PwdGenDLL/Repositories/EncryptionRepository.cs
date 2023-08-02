using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        public void Add(Encryption entity)
        {
            _dbSet.Add(entity);
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Detached; throw; }
        }

        public void Update(Encryption entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            try { _dbContext.SaveChanges(); }
            catch { _dbContext.Entry(entity).State = EntityState.Unchanged; throw; }
        }

        public void Delete(Encryption entity)
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