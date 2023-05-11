namespace Contracts
{
    public interface IRepositoryWrapper
    {
        ICategoryRepository Category { get; }

        IContentRepository Content { get; }
        IPostRepository Post { get; }

        IPostCategoryRepository PostCategory {get;}

        IImageRepository Image { get; }

        IUnitOfWork UnitOfWork { get; }
    }
}
