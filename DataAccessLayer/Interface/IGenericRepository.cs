using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Common;

namespace DataAccessLayer.Interface
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> condition);
        Task<ICollection<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<T> GetByProperty(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> GetListByProperty(Expression<Func<T, bool>> predicate);
        Task<PaginatedResult<T>> GetPaginatedListAsync(int page, int pageSize);
    }
}
