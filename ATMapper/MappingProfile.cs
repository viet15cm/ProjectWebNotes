using AutoMapper;
using Dto;
using Entities.Models;

namespace ATMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryForWithDetailDto>();
            CreateMap<CategoryForWithDetailDto, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryForUpdateDto>();

            CreateMap<Category, CategoryForCreationDto>();

            CreateMap<Post, PostDto>();

            // All other mappings goes here
        }
    }
}
