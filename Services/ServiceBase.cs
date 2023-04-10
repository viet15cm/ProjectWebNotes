using Contracts;
namespace Services
{
    public class ServiceBase
    {
        protected readonly IRepositoryWrapper _repositoryManager;
       
        public ServiceBase(IRepositoryWrapper repositoryManager)
        {
            _repositoryManager = repositoryManager;

        }
    }
}
