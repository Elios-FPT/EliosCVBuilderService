using CVBuilder.Contract.Shared;
using CVBuilder.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Test
{
    public class GetUserCvByIdTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public GetUserCvByIdTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new UserCvsController(_senderMock.Object);
        }

        private void SetupHttpContext(string userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Auth-Request-User"] = userId;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetUserCv_ValidIdAndOwner_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var jsonString = """
            {
                "personalInfo": {
                    "firstName": "John",
                    "lastName": "Doe",
                    "email": "john@example.com"
                }
            }
            """;
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);

            var expectedResponse = new BaseResponseDto<JsonElement>
            {
                Status = 200,
                Message = "CV retrieved successfully",
                ResponseData = jsonElement
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(cvId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("CV retrieved successfully", result.Message);
            Assert.NotNull(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.Is<GetUserCvByIdQuery>(q =>
                    q.IdHeader == userId &&
                    q.Id == cvId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task GetUserCv_ValidIdButNotOwner_ReturnsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<JsonElement>
            {
                Status = 403,
                Message = "You do not own this CV",
                ResponseData = default
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(cvId);

            // Assert
            Assert.Equal(403, result.Status);
            Assert.Contains("You do not own this CV", result.Message);

            _senderMock.Verify(s => s.Send(
                It.Is<GetUserCvByIdQuery>(q =>
                    q.IdHeader == userId &&
                    q.Id == cvId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task GetUserCv_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<JsonElement>
            {
                Status = 404,
                Message = "CV not found",
                ResponseData = default
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(cvId);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("CV not found", result.Message);

            _senderMock.Verify(s => s.Send(
                It.Is<GetUserCvByIdQuery>(q =>
                    q.IdHeader == userId &&
                    q.Id == cvId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task GetUserCv_MissingHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext(null);
            var cvId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _controller.GetUserCv(cvId)
            );
        }

        [Fact]
        public async Task GetUserCv_InvalidHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext("not-a-guid");
            var cvId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(
                async () => await _controller.GetUserCv(cvId)
            );
        }

        [Fact]
        public async Task GetUserCv_HandlerException_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<JsonElement>
            {
                Status = 500,
                Message = "Internal server error",
                ResponseData = default
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(cvId);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Internal server error", result.Message);

            _senderMock.Verify(s => s.Send(
                It.IsAny<GetUserCvByIdQuery>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }
    }
}
