using CVBuilder.Contract.TransferObjects;
using CVBuilder.Domain.Entities;
using System.Linq;

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
            // Deserialize Data JSON directly to DTO
            if (string.IsNullOrEmpty(userCv.Data))
            {
                return null!;
            }

            return System.Text.Json.JsonSerializer.Deserialize<UserCvDto>(
                userCv.Data, 
                new System.Text.Json.JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase 
                }) ?? null!;
        }
    }
}
