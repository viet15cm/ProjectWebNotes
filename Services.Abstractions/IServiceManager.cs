namespace Services.Abstractions
{
    public interface IServiceManager
    {
        ICategoryService CategoryService { get; }

        IPostService PostService { get; }

        IPostCategoryService PostCategoryService { get; }

        IContentService ContentService { get; }

        IImageService ImageService { get; }

        IHttpClientServiceImplementation HttpClient { get; }

    }
}
