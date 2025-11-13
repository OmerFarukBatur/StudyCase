using Core.Entities;
using Core.Interfaces.IRepositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
