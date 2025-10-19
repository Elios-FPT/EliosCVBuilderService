using CVBuilder.Contract.Shared;
using CVBuilder.Web.Controllers;
using MediatR;
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

        [Fact]
        public async Task DeleteUserCv_ValidId_ReturnsSuccess()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<bool>
            {
                Status = 200,
                Message = "User CV deleted successfully",
                ResponseData = true
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(userCvId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV deleted successfully", result.Message);
            Assert.True(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<DeleteUserCvCommand>(cmd =>
                cmd.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteUserCv_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<bool>
            {
                Status = 404,
                Message = "User CV not found",
                ResponseData = false
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(userCvId);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV not found", result.Message);
            Assert.False(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<DeleteUserCvCommand>(cmd =>
                cmd.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteUserCv_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<bool>
            {
                Status = 500,
                Message = "Failed to delete user CV: Database connection error",
                ResponseData = false
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteUserCv(userCvId);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to delete user CV", result.Message);
            Assert.False(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<DeleteUserCvCommand>(cmd =>
                cmd.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
