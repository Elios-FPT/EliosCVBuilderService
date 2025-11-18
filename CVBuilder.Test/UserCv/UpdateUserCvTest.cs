using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
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
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Test
{
    public class UpdateUserCvTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public UpdateUserCvTest()
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
        public async Task UpdateUserCv_ValidRequestAndOwner_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var jsonString = """{"personalInfo": {"firstName": "Jane"}}""";
            var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDoc.RootElement;
            var expectedBody = JsonSerializer.Serialize(jsonElement);

            var expectedResponse = new BaseResponseDto<UpdateUserCvResponseDto>
            {
                Status = 200,
                Message = "CV updated successfully",
                ResponseData = new UpdateUserCvResponseDto(true, "Update successful")
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(cvId, jsonElement);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("CV updated successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.True(result.ResponseData.Success);

            _senderMock.Verify(s => s.Send(
                It.Is<UpdateUserCvCommand>(cmd =>
                    cmd.Id == cvId &&
                    cmd.IdHeader == userId &&
                    cmd.Body == expectedBody
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task UpdateUserCv_ValidRequestButNotOwner_ReturnsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var jsonString = """{"personalInfo": {"firstName": "Jane"}}""";
            var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDoc.RootElement;

            var expectedResponse = new BaseResponseDto<UpdateUserCvResponseDto>
            {
                Status = 403,
                Message = "You do not own this CV",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(cvId, jsonElement);

            // Assert
            Assert.Equal(403, result.Status);
            Assert.Contains("You do not own this CV", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.Is<UpdateUserCvCommand>(cmd =>
                    cmd.Id == cvId &&
                    cmd.IdHeader == userId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task UpdateUserCv_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var jsonString = """{"personalInfo": {"firstName": "Jane"}}""";
            var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDoc.RootElement;

            var expectedResponse = new BaseResponseDto<UpdateUserCvResponseDto>
            {
                Status = 404,
                Message = "CV not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(cvId, jsonElement);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("CV not found", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.IsAny<UpdateUserCvCommand>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task UpdateUserCv_MissingHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext(null);
            var cvId = Guid.NewGuid();

            var jsonString = """{"personalInfo": {"firstName": "Jane"}}""";
            var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDoc.RootElement;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _controller.UpdateUserCv(cvId, jsonElement)
            );
        }

        [Fact]
        public async Task UpdateUserCv_InvalidHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext("invalid");
            var cvId = Guid.NewGuid();

            var jsonString = """{"personalInfo": {"firstName": "Jane"}}""";
            var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDoc.RootElement;

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(
                async () => await _controller.UpdateUserCv(cvId, jsonElement)
            );
        }

        [Fact]
        public async Task UpdateUserCv_HandlerException_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var jsonString = """{"personalInfo": {"firstName": "Jane"}}""";
            var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDoc.RootElement;

            var expectedResponse = new BaseResponseDto<UpdateUserCvResponseDto>
            {
                Status = 500,
                Message = "Internal server error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(cvId, jsonElement);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Internal server error", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.IsAny<UpdateUserCvCommand>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }
    }
}
