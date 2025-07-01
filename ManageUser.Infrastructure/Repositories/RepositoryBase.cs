using ManageUser.Application.Repositories;
using ManageUser.Infrastructure.EntityPersistence;
using ManageUser.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageUser.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        // Constructor to initialize _context
        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity); // add to memory
            await _context.SaveChangesAsync(); // add to database
            return entity;
        }

        public void AddEntity(T entity)
        {
            _context.Set<T>().Add(entity); // add to memory
        }

        public void AddRange(List<T> entities)
        {
            _context.Set<T>().AddRange(entities); // add to memory
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity); // remove to memory
            await _context.SaveChangesAsync(); // add change to database
        }

        public void DeleteEntity(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IReadOnlyList<T> entities)
        {
            _context.Set<T>().RemoveRange(entities); // remove to memory
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, string? includeString, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return (await _context.Set<T>().FindAsync(id));
        }

        public async Task<T> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null or empty.");
            }
            var result = await _context.Set<T>().FindAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Entity of type {typeof(T).Name} not found with ID: {id}");
            }
            return result;
        }

        public async Task<T> GetEntityAsync(Expression<Func<T, bool>>? predicate, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
            if (predicate != null) query = query.Where(predicate);
            var result= (await query.FirstOrDefaultAsync());
            if (result == null)
            {
                throw new KeyNotFoundException($"Entity of type {typeof(T).Name} not found with the specified criteria.");
            }
            return result;
        }

        public async Task UpdateRangeAsync(List<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public void UpdateEntity(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<T> GetByIdWithSpec(ISpecification<T> spec)
        {
            var query = await ApplySpecification(spec).FirstOrDefaultAsync();
            return query!;
        }

        public async Task<IReadOnlyList<T>> GetAllByIdWithSpec(ISpecification<T> spec)
        {
            var query = await ApplySpecification(spec).ToListAsync();
            return query;
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            var query = await ApplySpecification(spec).CountAsync();
            return query;
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            var application = SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
            return application;
        }

        
    }
}

