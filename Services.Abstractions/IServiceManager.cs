namespace Services.Abstractions
{
    public interface IServiceManager
    {
        ICategoryService CategoryService { get; }

        IPostService PostService { get; }

        IPostCategoryService PostCategoryService { get; }


    }
}
