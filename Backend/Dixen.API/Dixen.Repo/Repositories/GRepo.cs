using Dixen.Repo.Model;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Repositories
{
    public class GRepo<T> : IGRepo<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _dbSet;

        public GRepo(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> Create(T item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> Delete(object id)
        {
            T? data = await _dbSet.FindAsync(id);
            if (data == null) return false;

            _dbSet.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<T>> GetAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();
            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }
        public async Task<T?> GetById(object id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == (int)id);
        }

        public async Task<bool> Exists(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            if (include != null)
                query = include(query);
            return await query.ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<T?> Update(object id, T item)
        {
            var existingItem = await _dbSet.FindAsync(id);
            if (existingItem == null) return null;

            // Update scalar properties only (ignore navigation props)
            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return existingItem;
        }
        public async Task<List<T>> GetAllForAnalytics(Func<IQueryable<T>, IQueryable<T>> configure)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();
            return await configure(query).ToListAsync();
        }


        public IQueryable<T> GetAllQuery()
        {
            return _dbSet.AsNoTracking();
        }

    }
}
