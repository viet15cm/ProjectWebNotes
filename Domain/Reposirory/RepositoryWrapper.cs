using Contracts;
using Domain.Reposirory;
using Entities.Models;

namespace Domain.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly  RepositoryContext _repoContext;

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }

        private IPostRepository _post;
        private ICategoryRepository _category;

        private IContentRepository _content;

        private IPostCategoryRepository _postCategory;

        private IUnitOfWork _unitOfWork;

        private IImageRepository _image;
        public IContentRepository Content
        {

            get
            {
                if (_content == null)
                {
                    _content = new ContentRepository(_repoContext);
                }

                return _content;
            }
        }

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_repoContext);
                }

                return _category;
            }
        }

        public IPostRepository Post
        {
            get
            {
                if (_post == null)
                {
                    _post = new PostRepository(_repoContext);
                }

                return _post;
            }

        }

      
        public IPostCategoryRepository PostCategory
        {
            get
            {
                if (_postCategory == null)
                {
                    _postCategory = new PostCategoryRepositoty(_repoContext);
                }

                return _postCategory;
            }
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                    _unitOfWork = new UnitOfWork(_repoContext);

                return _unitOfWork;
            }
        }

        public IImageRepository Image
        {
            get
            {
                if (_image == null)
                {
                    _image = new ImageRepository(_repoContext);
                }

                return _image;
            }
        }
    }
}
