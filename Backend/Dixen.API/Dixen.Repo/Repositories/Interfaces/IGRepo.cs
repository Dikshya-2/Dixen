using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Repositories.Interfaces
{
    public interface IGRepo<T> where T : class
    {
        Task<T> Create(T item);
        //Task Delete(object id);
        Task<bool> Delete(object id);

        Task<List<T>> GetAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task<T?> GetById(object id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task<bool> Exists(object id);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task Save();
        Task<T?> Update(object id,T item);
    }
}
