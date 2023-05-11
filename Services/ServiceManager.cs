using Contracts;
using Services.Abstractions;

namespace Services
{
    public class ServiceManager : IServiceManager
    {

        private readonly Lazy<ICategoryService> _lazyCategoyService;
        private readonly Lazy<IPostService> _lazyPostService;
        private readonly Lazy<IPostCategoryService> _lazyPostCategoryService;

        private readonly Lazy<IContentService> _lazyContentService;

        private readonly Lazy<IImageService> _lazyImageService;

        private readonly Lazy<IHttpClientServiceImplementation> _httpClientService;
        public ServiceManager(IRepositoryWrapper repositoryManager)
        {
            _lazyCategoyService = new Lazy<ICategoryService>(() => new CategoryService(repositoryManager));
            _lazyPostService = new Lazy<IPostService>(() => new PostService(repositoryManager));
            _lazyPostCategoryService = new Lazy<IPostCategoryService>(()=> new PostCategoryService(repositoryManager));
            
            _httpClientService = new Lazy<IHttpClientServiceImplementation>(()=> new HttpClientStreamService(new HttpClient()));

            _lazyContentService = new Lazy<IContentService>(() => new ContentService(repositoryManager));

            _lazyImageService = new Lazy<IImageService>(() => new ImageService(repositoryManager));
        }
        public IPostService PostService => _lazyPostService.Value;

        public IImageService ImageService => _lazyImageService.Value;
        public ICategoryService CategoryService =>  _lazyCategoyService.Value;


        public IContentService ContentService => _lazyContentService.Value;

        public IPostCategoryService PostCategoryService => _lazyPostCategoryService.Value;

        public IHttpClientServiceImplementation HttpClient => _httpClientService.Value;
    }
}
