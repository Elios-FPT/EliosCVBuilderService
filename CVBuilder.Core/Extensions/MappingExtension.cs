using CVBuilder.Contract.TransferObjects;
using CVBuilder.Domain.Entities;

namespace CVBuilder.Core.Extensions
{
    public static class MappingExtension
    {
        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                Url = notification.Url,
                Metadata = notification.Metadata
            };
        }

        public static CvTemplateDto ToDto(this Elios.CVBuilder.Domain.Models.TemplateCv templateCv)
        {
            return new CvTemplateDto
            {
                Id = templateCv.Id,
                Name = templateCv.Name,
                Description = templateCv.Description,
                ThumbnailUrl = templateCv.ThumbnailUrl,
                CreatedAt = templateCv.CreatedAt,
                IsDeleted = templateCv.IsDeleted
            };
        }

        public static UserCvDto ToDto(this Elios.CVBuilder.Domain.Models.UserCv userCv)
        {
            return new UserCvDto
            {
                Id = userCv.Id,
                UserId = userCv.UserId,
                TemplateId = userCv.TemplateId,
                Title = userCv.Title,
                CreatedAt = userCv.CreatedAt,
                UpdatedAt = userCv.UpdatedAt,
                Template = userCv.Template?.ToDto()
            };
        }
    }
}
