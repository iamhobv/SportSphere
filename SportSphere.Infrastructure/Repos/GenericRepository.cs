using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Infrastructure.Repos
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => e.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.DeletedAt == null && e.Id == id);
        }

        public async Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>>? expression)
        {
            IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);

            if (expression is not null)
                query = query.Where(expression);

            return await query.ToListAsync();
        }


        public async Task<T> GetFilteredFirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);

            if (expression != null)
                query = query.Where(expression);

            return await query.FirstOrDefaultAsync(expression);
        }
        public async Task<T> GetFilteredFirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null).AsNoTracking();

            if (expression != null)
                query = query.Where(expression).AsNoTracking();

            return await query.FirstOrDefaultAsync(expression);
        }




        public async Task<IEnumerable<T>> GetFilteredOrderedAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);

            if (filter != null)
                query = query.Where(filter);

            query = orderBy != null
                ? orderBy(query)
                : query.OrderBy(e => e.Id);

            return await query.ToListAsync();
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);

            if (filter != null)
                query = query.Where(filter);

            int totalCount = await query.CountAsync();

            query = orderBy != null
                ? orderBy(query)
                : query.OrderBy(e => e.Id);

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
.ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> QueryAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            const int DefaultPageSize = 10;

            IQueryable<T> query = _dbSet.Where(e => e.DeletedAt == null);

            if (filter != null)
                query = query.Where(filter);

            int totalCount = await query.CountAsync();

            query = orderBy != null
                ? orderBy(query)
                : query.OrderBy(e => e.Id);

            if (pageIndex.HasValue)
            {
                int size = pageSize ?? DefaultPageSize;
                query = query
                   .Skip((pageIndex.Value - 1) * size)
                   .Take(size);
            }

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.Where(e => e.DeletedAt == null);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            return true;
        }

        public void Delete(T entity)
        {
            entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void ActualDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

    }

}
