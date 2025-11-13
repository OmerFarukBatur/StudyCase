using Core.Entities.Common;
using Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Interfaces.IRepositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table { get; }
        IQueryable<T> GetAll(bool tracking = true);
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);
        Task<T> GetByIdAsync(int id, bool tracking = true);

        Task<bool> AddAsync(T model);
        Task<bool> AddRangeAsync(List<T> datas);
        bool Remove(T model);
        bool RemoveRange(List<T> datas);
        Task<bool> RemoveAsync(int id);
        bool Update(T model);
        Task<int> SaveAsync();

        Task<PaginatedList<T>> GetPagedAsync(
        Expression<Func<T, bool>> filter = null,
        int pageIndex = 1,
        int pageSize = 20,
        bool tracking = false);
    }
}
