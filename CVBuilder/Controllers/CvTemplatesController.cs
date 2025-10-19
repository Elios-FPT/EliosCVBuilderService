using Asp.Versioning;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static CVBuilder.Contract.UseCases.CvTemplate.Command;
using static CVBuilder.Contract.UseCases.CvTemplate.Query;
using static CVBuilder.Contract.UseCases.CvTemplate.Request;

namespace CVBuilder.Web.Controllers
{
    /// <summary>
    /// CV Template management endpoints.
    /// </summary>
    [ApiVersion(1)]
    [Produces("application/json")]
    [ControllerName("CvTemplates")]
    [Route("api/v1/[controller]")]
    public class CvTemplatesController : ControllerBase
    {
        private readonly ISender _sender;

        public CvTemplatesController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new CV template.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `template:write` permission to create a new CV template.
        /// </pre>
        /// </remarks>
        /// <param name="request">A <see cref="CreateCvTemplateRequest"/> object containing the template details.</param>
        /// <returns>
        /// → <seealso cref="CreateCvTemplateCommand" /><br/>
        /// → <seealso cref="CreateCvTemplateCommandHandler" /><br/>
        /// → A <see cref="BaseResponseDto{CvTemplateDto}"/> containing the created template.<br/>
        /// </returns>
        /// <response code="200">Template created successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseDto<CvTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<BaseResponseDto<CvTemplateDto>> CreateTemplate([FromBody] CreateCvTemplateRequest request)
        {
            var command = new CreateCvTemplateCommand(
                Name: request.Name,
                Description: request.Description,
                ThumbnailUrl: request.ThumbnailUrl);

            return await _sender.Send(command);
        }

        /// <summary>
        /// Retrieves a specific CV template by its ID.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `template:read` permission to retrieve a specific CV template.
        /// </pre>
        /// </remarks>
        /// <param name="id">The unique identifier of the CV template.</param>
        /// <returns>
        /// → <seealso cref="GetCvTemplateByIdQuery" /><br/>
        /// → <seealso cref="GetCvTemplateByIdQueryHandler" /><br/>
        /// → A <see cref="BaseResponseDto{CvTemplateDto}"/> containing the template.<br/>
        /// </returns>
        /// <response code="200">Template retrieved successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified template was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<CvTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<CvTemplateDto>> GetTemplate([FromRoute] Guid id)
        {
            var query = new GetCvTemplateByIdQuery(Id: id);
            return await _sender.Send(query);
        }

        /// <summary>
        /// Retrieves a paginated list of CV templates.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `template:read` permission to retrieve a paginated list of CV templates.
        /// </pre>
        /// </remarks>
        /// <param name="request">A <see cref="GetAllCvTemplatesRequest"/> object containing pagination parameters.</param>
        /// <returns>
        /// → <seealso cref="GetAllCvTemplatesQuery" /><br/>
        /// → <seealso cref="GetAllCvTemplatesQueryHandler" /><br/>
        /// → A <see cref="BaseResponseDto{IEnumerable{CvTemplateDto}}"/> containing the templates.<br/>
        /// </returns>
        /// <response code="200">Templates retrieved successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseDto<IEnumerable<CvTemplateDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<BaseResponseDto<IEnumerable<CvTemplateDto>>> GetTemplates([FromQuery] GetAllCvTemplatesRequest request)
        {
            var query = new GetAllCvTemplatesQuery(
                PageNumber: request.PageNumber,
                PageSize: request.PageSize,
                IncludeDeleted: request.IncludeDeleted);

            return await _sender.Send(query);
        }

        /// <summary>
        /// Updates an existing CV template.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `template:write` permission to update an existing CV template.
        /// </pre>
        /// </remarks>
        /// <param name="id">The unique identifier of the CV template to update.</param>
        /// <param name="request">A <see cref="UpdateCvTemplateRequest"/> object containing the updated template details.</param>
        /// <returns>
        /// → <seealso cref="UpdateCvTemplateCommand" /><br/>
        /// → <seealso cref="UpdateCvTemplateCommandHandler" /><br/>
        /// → A <see cref="BaseResponseDto{CvTemplateDto}"/> containing the updated template.<br/>
        /// </returns>
        /// <response code="200">Template updated successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified template was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<CvTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<CvTemplateDto>> UpdateTemplate([FromRoute] Guid id, [FromBody] UpdateCvTemplateRequest request)
        {
            var command = new UpdateCvTemplateCommand(
                Id: id,
                Name: request.Name,
                Description: request.Description,
                ThumbnailUrl: request.ThumbnailUrl);

            return await _sender.Send(command);
        }

        /// <summary>
        /// Soft deletes a CV template.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `template:delete` permission to soft delete a CV template.
        /// </pre>
        /// </remarks>
        /// <param name="id">The unique identifier of the CV template to delete.</param>
        /// <returns>
        /// → <seealso cref="DeleteCvTemplateCommand" /><br/>
        /// → <seealso cref="DeleteCvTemplateCommandHandler" /><br/>
        /// → A <see cref="BaseResponseDto{bool}"/> indicating success.<br/>
        /// </returns>
        /// <response code="200">Template deleted successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified template was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<bool>> DeleteTemplate([FromRoute] Guid id)
        {
            var command = new DeleteCvTemplateCommand(Id: id);
            return await _sender.Send(command);
        }
    }
}
