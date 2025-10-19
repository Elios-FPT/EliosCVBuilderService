using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.UseCases.UserCv
{
    public static class Request
    {
        public record CreateUserCvRequest(
            [Required] Guid UserId,
            [Required] Guid TemplateId,
            [Required] string Title
        );

        public record UpdateUserCvRequest(
            [Required] string Title
        );

        public record GetUserCvsRequest(
            [Required] Guid UserId,
            int PageNumber = 1,
            int PageSize = 20
        );
    }
}
