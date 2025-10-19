using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.UseCases.CvTemplate
{
    public static class Request
    {
        public record CreateCvTemplateRequest(
            [Required] string Name,
            string? Description,
            string? ThumbnailUrl
        );

        public record UpdateCvTemplateRequest(
            [Required] string Name,
            string? Description,
            string? ThumbnailUrl
        );

        public record GetAllCvTemplatesRequest(
            int PageNumber = 1,
            int PageSize = 20,
            bool IncludeDeleted = false
        );
    }
}
