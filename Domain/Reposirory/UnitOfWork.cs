using Contracts;
using Domain.Reposirory;

namespace Domain.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private RepositoryContext RepositoryContext { get; set; }
        public UnitOfWork(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await RepositoryContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return RepositoryContext.SaveChanges();
        }
    }
}
