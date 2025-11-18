using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Command;
using static CVBuilder.Contract.UseCases.UserCv.Request;

namespace CVBuilder.Test
{
    public class CreateUserCvTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public CreateUserCvTest()
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
        public async Task CreateUserCv_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var request = new CreateUserCvRequest("My Resume");
            var expectedResponse = new BaseResponseDto<CreateUserCvResponseDto>
            {
                Status = 201,
                Message = "User CV created successfully",
                ResponseData = new CreateUserCvResponseDto(cvId)
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.Status);
            Assert.Equal("User CV created successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(cvId, result.ResponseData.Id);

            _senderMock.Verify(s => s.Send(
                It.Is<CreateUserCvCommand>(cmd =>
                    cmd.OwnerId == userId &&
                    cmd.ResumeTitle == "My Resume"
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task CreateUserCv_MissingHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext(null);
            var request = new CreateUserCvRequest("My Resume");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _controller.CreateUserCv(request)
            );
        }

        [Fact]
        public async Task CreateUserCv_InvalidHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext("invalid-guid-format");
            var request = new CreateUserCvRequest("My Resume");

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(
                async () => await _controller.CreateUserCv(request)
            );
        }

        [Fact]
        public async Task CreateUserCv_EmptyResumeTitle_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var request = new CreateUserCvRequest("");
            var expectedResponse = new BaseResponseDto<CreateUserCvResponseDto>
            {
                Status = 400,
                Message = "Resume title is required",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Contains("Resume title is required", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.Is<CreateUserCvCommand>(cmd =>
                    cmd.OwnerId == userId &&
                    cmd.ResumeTitle == ""
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task CreateUserCv_HandlerException_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var request = new CreateUserCvRequest("My Resume");
            var expectedResponse = new BaseResponseDto<CreateUserCvResponseDto>
            {
                Status = 500,
                Message = "Database connection failed",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Database connection failed", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.IsAny<CreateUserCvCommand>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }
    }
}
