using AutoMapper;
using Dto;
using Entities.Models;

namespace ATMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryForUpdateDto>();
            CreateMap<Category, CategoryForCreationDto>();
            CreateMap<CategoryForCreationDto, Category>();

            CreateMap<Post, PostDto>();

            CreateMap<Post, PostForCreationDto>();

            CreateMap<Post, PostForUpdateDto>();

            CreateMap<PostForCreationDto, Post>();
            CreateMap<Post, PostFWDetailChilds>();
          
            CreateMap<Post, PostSelectDto>();

            CreateMap<Post, PostForUpdateContentDto>();

            CreateMap<Image, ImageDto>();

            CreateMap<Image, ImageForCreateDto>();

            CreateMap<ImageForCreateDto,Image>();

            CreateMap<Post, PostsFWDImagesDto>()
                .ForMember(x => x.Images, y => y.MapFrom(k => k.Images));

            CreateMap<Post, PostsFWDContentImagesDto>()
                .ForMember(x => x.Images, y => y.MapFrom(k => k.Images))
                .ForMember(x => x.Contents, y => y.MapFrom(k => k.Contents));


            CreateMap<Content, ContentDto>();

            CreateMap<ContentDto, Content>();

            CreateMap<Content, ContentFWDPost>()
                .ForMember(x => x.Post , y => y.MapFrom(k => k.Post));

            CreateMap<Content, ContentForCreateDto>();

            CreateMap<Content, ContentForUpdateDto>();

            CreateMap<Content, ContentFWDChildsDto>();

            CreateMap<ContentForCreateDto, Content>();

        }
    }
}
