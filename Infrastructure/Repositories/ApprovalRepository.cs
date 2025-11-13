using Core.Entities;
using Core.Interfaces.IRepositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ApprovalRepository : Repository<Approval>, IApprovalRepository
    {
        public ApprovalRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
