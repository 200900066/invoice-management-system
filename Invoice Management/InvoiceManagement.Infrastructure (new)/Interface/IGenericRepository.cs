using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null,Func<IQueryable<T>, IQueryable<T>> include = null);

        Task<T> GetSingleAsync( Expression<Func<T, bool>> predicate,Func<IQueryable<T>, IQueryable<T>> include = null);

        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task<T> Update(T entity);
        void Delete(T entity);
    }
}
