using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPostCategoryService
    {
        Task<IEnumerable<PostCategoryForWithDetailDto>> GetByIdCategoryWithDetailAsync(string categoryId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PostCategoryForWithDetailDto>> GetByIdPostWithDetailAsync(string postId, CancellationToken cancellationToken = default);
    }
}
