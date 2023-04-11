using AutoMapper;
using Dto;
using Entities.Models;

namespace ATMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryForWithDetailDto>()
                .ForMember(x => x.PostCategories, k => k.MapFrom(y => y.PostCategories));

            CreateMap<PostCategory, PostCategoryForWithDetailDto>()
                .ForMember(x => x.Category, k => k.MapFrom(y => y.Category))
                .ForMember(x => x.Post, k => k.MapFrom(y => y.Post));

            CreateMap<Post, PostForWithDetailDto>()
                .ForMember(x => x.PostCategories, k => k.MapFrom(y => y.PostCategories));

            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryForUpdateDto>();
            CreateMap<Category, CategoryForCreationDto>();
            CreateMap<Post, PostDto>();

            // All other mappings goes here
        }
    }
}
