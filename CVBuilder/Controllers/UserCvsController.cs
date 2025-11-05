using Asp.Versioning;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using static CVBuilder.Contract.UseCases.UserCv.Command;
using static CVBuilder.Contract.UseCases.UserCv.Query;
using static CVBuilder.Contract.UseCases.UserCv.Request;

namespace CVBuilder.Web.Controllers
{
    /// <summary>
    /// User CV management endpoints.
    /// </summary>
    [ApiVersion(1)]
    [Produces("application/json")]
    [ControllerName("CVBuilder/UserCvs")]
    [Route("api/cvbuilder/[controller]")]
    public class UserCvsController : ControllerBase
    {
        private readonly ISender _sender;

        public UserCvsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new user CV.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `usercv:write` permission to create a new user CV.
        /// </pre>
        /// </remarks>
        /// <param name="request">A <see cref="CreateUserCvRequest"/> object containing the user CV details.</param>
        /// <returns>
        /// → <seealso cref="CreateUserCvCommandV2" /><br/>
        /// → <seealso cref="CreateUserCvCommandV2Handler" /><br/>
        /// → A <see cref="BaseResponseDto{UserCvDto}"/> containing the created user CV.<br/>
        /// </returns>
        /// <response code="200">User CV created successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified template was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseDto<UserCvDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<UserCvDto>> CreateUserCv([FromBody] JsonElement request)
        {
            var userIdHeader = HttpContext.Request.Headers["X-Auth-Request-User"].FirstOrDefault();
            Guid id = Guid.Parse(userIdHeader);
            var Body = System.Text.Json.JsonSerializer.Serialize(request);
            var command = new CreateUserCvCommand(
                Id: id,
                Body: Body);
            return await _sender.Send(command);
        }

        /// <summary>
        /// Retrieves a specific user CV by its ID.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `usercv:read` permission to retrieve a specific user CV.
        /// </pre>
        /// </remarks>
        /// <param name="id">The unique identifier of the user CV.</param>
        /// <returns>
        /// → <seealso cref="GetUserCvByIdQuery" /><br/>
        /// → <seealso cref="GetUserCvByIdQueryHandler" /><br/>
        /// → A <see cref="BaseResponseDto{UserCvDto}"/> containing the user CV.<br/>
        /// </returns>
        /// <response code="200">User CV retrieved successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified user CV was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<UserCvDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<UserCvDto>> GetUserCv([FromRoute] Guid id)
        {
            var query = new GetUserCvByIdQuery(Id: id);
            return await _sender.Send(query);
        }

        /// <summary>
        /// Retrieves a paginated list of user CVs for a specific user.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `usercv:read` permission to retrieve a paginated list of user CVs.
        /// </pre>
        /// </remarks>
        /// <param name="request">A <see cref="GetUserCvsRequest"/> object containing pagination parameters and user ID.</param>
        /// <returns>
        /// → <seealso cref="GetUserCvsQuery" /><br/>
        /// → <seealso cref="GetUserCvsQueryHandler" /><br/>
        /// → A <see cref="BaseResponseDto{IEnumerable{UserCvDto}}"/> containing the user CVs.<br/>
        /// </returns>
        /// <response code="200">User CVs retrieved successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseDto<IEnumerable<UserCvDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<BaseResponseDto<IEnumerable<UserCvDto>>> GetUserCvs([FromQuery] GetUserCvsRequest request)
        {
            var query = new GetUserCvsQuery(
                UserId: request.UserId,
                PageNumber: request.PageNumber,
                PageSize: request.PageSize);

            return await _sender.Send(query);
        }

        /// <summary>
        /// Updates an existing user CV.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `usercv:write` permission to update an existing user CV.
        /// </pre>
        /// </remarks>
        /// <param name="id">The unique identifier of the user CV to update.</param>
        /// <param name="request">A <see cref="UpdateUserCvRequestV2"/> object containing the updated user CV details.</param>
        /// <returns>
        /// → <seealso cref="UpdateUserCvCommandV2" /><br/>
        /// → <seealso cref="UpdateUserCvCommandV2Handler" /><br/>
        /// → A <see cref="BaseResponseDto{UserCvDto}"/> containing the updated user CV.<br/>
        /// </returns>
        /// <response code="200">User CV updated successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified user CV was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<UserCvDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<UserCvDto>> UpdateUserCv([FromRoute] Guid id, [FromBody] UpdateUserCvRequestV2 request)
        {
            var command = new UpdateUserCvCommandV2(
                Id: id,
                PersonalInfo: request.PersonalInfo,
                Experience: request.Experience,
                Projects: request.Projects,
                Education: request.Education,
                Skillsets: request.Skillsets);
            return await _sender.Send(command);
        }

        /// <summary>
        /// Deletes a user CV.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// Description:
        /// This endpoint allows authenticated users with the `usercv:delete` permission to delete a user CV.
        /// </pre>
        /// </remarks>
        /// <param name="id">The unique identifier of the user CV to delete.</param>
        /// <returns>
        /// → <seealso cref="DeleteUserCvCommand" /><br/>
        /// → <seealso cref="DeleteUserCvCommandHandler" /><br/>
        /// → A <see cref="BaseResponseDto{bool}"/> indicating success.<br/>
        /// </returns>
        /// <response code="200">User CV deleted successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to access this resource.</response>
        /// <response code="404">The specified user CV was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<BaseResponseDto<bool>> DeleteUserCv([FromRoute] Guid id)
        {
            var command = new DeleteUserCvCommand(Id: id);
            return await _sender.Send(command);
        }
    }
}
