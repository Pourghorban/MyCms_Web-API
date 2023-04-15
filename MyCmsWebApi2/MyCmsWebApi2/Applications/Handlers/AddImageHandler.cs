﻿using MediatR;
using MyCmsWebApi2.Applications.Commands;
using MyCmsWebApi2.Applications.Repository;
using MyCmsWebApi2.Domain.Entities;
using MyCmsWebApi2.Infrastructure.Convertor;
using MyCmsWebApi2.Infrastructure.Exceptions.BaseException;

namespace MyCmsWebApi2.Applications.Handlers
{
    public class AddImageHandler : IRequestHandler<AddImageCommand, Guid>
    {
        private readonly IImageRepository _imageRepository;

        public AddImageHandler(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<Guid> Handle(AddImageCommand request, CancellationToken cancellationToken)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
                throw new PhoenixGeneralException("حاجی یه عکسی انتخاب کن لطفا");
            if (request.NewsGroupId == null && request.NewsId == null)
            {
                throw new PhoenixGeneralException("حاجی یه خبر یا گروه انتخاب کن لطفا");
            }
            else if (request.NewsGroupId != null && request.NewsId != null)
            {
                throw new PhoenixGeneralException("حاجی یدونه انتخاب کن لطفا");
            }

            var image = new Images()
            {
                Base64 = request.ImageFile.ImageToBase64(),
                CreateDate = DateTime.Now,
                Id = Guid.NewGuid(),
                ImageName = request.ImageFile.Name,
                NewsGroupId = request.NewsGroupId,
                NewsId = request.NewsId,
                ContentType = request.ImageFile.ContentType
            };
            var result = await _imageRepository.InsertImageAsync(image);
            return result.Id;
        }
    }
}