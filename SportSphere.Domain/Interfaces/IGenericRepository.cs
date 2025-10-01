using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> expression);
        Task<T> GetFilteredFirstOrDefaultAsync(Expression<Func<T, bool>> expression);
        Task<T> GetFilteredFirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> expression);

        Task<IEnumerable<T>> GetFilteredOrderedAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        Task<(IEnumerable<T> Items, int TotalCount)> QueryAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? pageIndex = null,
        int? pageSize = null);

        IQueryable<T> GetQueryable();
        Task AddAsync(T entity);
        void Update(T entity);
        Task<bool> DeleteAsync(int id);
        void Delete(T entity);
        Task<bool> SaveChangesAsync();

        void ActualDelete(T entity);
        public Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);

    }

}
