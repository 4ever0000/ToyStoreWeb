using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToyStore.Data;

namespace ToyStore.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);

            // T-nin ID-si olduğunu fərz edirik (Entity-də Id olmalıdır)
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;
        public void Delete(T entity) => _dbSet.Remove(entity);
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}