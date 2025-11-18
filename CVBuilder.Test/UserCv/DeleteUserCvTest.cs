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

namespace CVBuilder.Test
{
    public class DeleteUserCvTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public DeleteUserCvTest()
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
        public async Task DeleteUserCv_ValidIdAndOwner_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<DeleteUserCvResponseDto>
            {
                Status = 200,
                Message = "CV deleted successfully",
                ResponseData = new DeleteUserCvResponseDto(true, "Delete successful")
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(cvId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("CV deleted successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.True(result.ResponseData.Success);

            _senderMock.Verify(s => s.Send(
                It.Is<DeleteUserCvCommand>(cmd =>
                    cmd.IdHeader == userId &&
                    cmd.Id == cvId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task DeleteUserCv_ValidIdButNotOwner_ReturnsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<DeleteUserCvResponseDto>
            {
                Status = 403,
                Message = "You do not own this CV",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(cvId);

            // Assert
            Assert.Equal(403, result.Status);
            Assert.Contains("You do not own this CV", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.Is<DeleteUserCvCommand>(cmd =>
                    cmd.IdHeader == userId &&
                    cmd.Id == cvId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task DeleteUserCv_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<DeleteUserCvResponseDto>
            {
                Status = 404,
                Message = "CV not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(cvId);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("CV not found", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.IsAny<DeleteUserCvCommand>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task DeleteUserCv_MissingHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext(null);
            var cvId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _controller.DeleteUserCv(cvId)
            );
        }

        [Fact]
        public async Task DeleteUserCv_InvalidHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext("invalid-guid-format");
            var cvId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(
                async () => await _controller.DeleteUserCv(cvId)
            );
        }

        [Fact]
        public async Task DeleteUserCv_HandlerException_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<DeleteUserCvResponseDto>
            {
                Status = 500,
                Message = "Internal server error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(cvId);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Internal server error", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.IsAny<DeleteUserCvCommand>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }
    }
}
