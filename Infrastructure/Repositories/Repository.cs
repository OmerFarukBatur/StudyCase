using Core.Entities.Common;
using Core.Helpers;
using Core.Interfaces.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _table;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public DbSet<T> Table => _table;

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = _table.AsQueryable();
            return tracking ? query : query.AsNoTracking();
        }

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = _table.Where(method);
            return tracking ? query : query.AsNoTracking();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = _table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(method);
        }

        public async Task<T> GetByIdAsync(int id, bool tracking = true)
        {
            var query = _table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddAsync(T model)
        {
            await _table.AddAsync(model);
            return true;
        }

        public async Task<bool> AddRangeAsync(List<T> datas)
        {
            await _table.AddRangeAsync(datas);
            return true;
        }

        public bool Remove(T model)
        {
            _table.Remove(model);
            return true;
        }

        public bool RemoveRange(List<T> datas)
        {
            _table.RemoveRange(datas);
            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            return Remove(entity);
        }

        public bool Update(T model)
        {
            _table.Update(model);
            return true;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<T>> GetPagedAsync(
        Expression<Func<T, bool>> filter = null,
        int pageIndex = 1,
        int pageSize = 10,
        bool tracking = false)
        {
            IQueryable<T> query = _table;

            if (filter != null)
                query = query.Where(filter);

            if (!tracking)
                query = query.AsNoTracking();
            return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
