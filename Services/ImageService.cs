using ATMapper;
using Contracts;
using Dto;
using Entities.Models;
using Exceptions;
using ExtentionLinqEntitys;
using Services.Abstractions;
using System.Security.Cryptography;

namespace Services
{
    public class ImageService : ServiceBase, IImageService
    {
        public ImageService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
        }

        public async Task<ImageDto> CreateAsync(ImageForCreateDto imageForCreate)
        {
            if (imageForCreate is null)
            {
                throw new ObjectNullException("không có đối tượng để tạo.");
            }

            var image = ObjectMapper.Mapper.Map< Image>(imageForCreate);

            _repositoryManager.Image.Create(image);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<ImageDto>(image); 
        }

        public async Task<ImageDto> CreateAsync(ImageForCreateDto imageForCreate, CancellationToken cancellationToken = default)
        {
            if (imageForCreate is null)
            {
                throw new ObjectNullException("không có đối tượng để tạo.");
            }

            var image = ObjectMapper.Mapper.Map<Image>(imageForCreate);

            _repositoryManager.Image.Create(image);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);


            return  ObjectMapper.Mapper.Map<ImageDto>(image);
        }

        public async Task<IEnumerable<ImageDto>> CreateFromAsync(IEnumerable<ImageForCreateDto> imageForCreateDtos , CancellationToken cancellationToken = default)
        {
            if (imageForCreateDtos is null)
            {
                throw new ObjectNullException("không có đối tượng để tạo.");
            }

            var images = ObjectMapper.Mapper.Map<IEnumerable<Image>>(imageForCreateDtos);

            foreach (var item in images)
            {
                _repositoryManager.Image.Create(item);

            }

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);


            return ObjectMapper.Mapper.Map<IEnumerable<ImageDto>>(images);


        }

      

        public async Task<ImageDto> DeleteAsync(int idImage, CancellationToken cancellationToken = default)
        {
            var img = await _repositoryManager.Image.GetByIdAsync(idImage, null, cancellationToken);

            if (img is null)
            {
                throw new ObjectNullException("Không có đối tượng để xóa.");
            }

            _repositoryManager.Image.Delete(img);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<ImageDto>(img);
        }


        public Task<bool> DeleteFormImagesAsync(IList<int> idImage, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public  async Task<Image> GetByIdAsync(int idImage ,ExpLinqEntity<Image> expLinqEntity , CancellationToken cancellationToken = default)
        {
            var image = await _repositoryManager.Image.GetByIdAsync(idImage, expLinqEntity , cancellationToken);

            if (image is null)
            {
                throw new ObjectNullException("Không tìm thấy đối tượng.");
            }

            return image;
        }

    }
}
