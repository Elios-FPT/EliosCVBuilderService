using System;
using System.Threading;
using System.Threading.Tasks;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Handler.UserCv.Command;
using CVBuilder.Core.Interfaces;
using Elios.CVBuilder.Domain.Models;
using Moq;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Test
{
    public class UpdateUserCvTest
    {
        private readonly Mock<IGenericRepository<UserCv>> _resumeRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateUserCvCommandHandler _handler;

        public UpdateUserCvTest()
        {
            _resumeRepoMock = new Mock<IGenericRepository<UserCv>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new UpdateUserCvCommandHandler(_resumeRepoMock.Object);
        }

        private void SetupTransaction()
        {
            _resumeRepoMock
                .Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessAndUpdatesCV()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "Original Title",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var newData = "{\"personalInfo\": {\"firstName\": \"Jane\"}}";
            var command = new UpdateUserCvCommand(cvId, userId, newData);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            SetupTransaction();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV updated successfully.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.True(result.ResponseData.Success);
            Assert.Equal("Resume updated successfully.", result.ResponseData.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);

            _resumeRepoMock.Verify(r => r.UpdateAsync(
                It.Is<UserCv>(cv =>
                    cv.Id == cvId &&
                    cv.Data == newData &&
                    cv.UpdatedAt > existingCv.CreatedAt)),
                Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyIdHeader_ReturnsBadRequest()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var command = new UpdateUserCvCommand(cvId, Guid.Empty, "{\"data\": \"test\"}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User ID cannot be empty.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Never);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyId_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCvCommand(Guid.Empty, userId, "{\"data\": \"test\"}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User CV ID cannot be empty.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Never);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_InvalidBody_ReturnsBadRequest(string body)
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateUserCvCommand(cvId, userId, body);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("Data is required.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Never);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NonExistentCv_ReturnsNotFound()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateUserCvCommand(cvId, userId, "{\"data\": \"test\"}");

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync((UserCv)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV not found.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbidden()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = differentUserId, // Different owner
                ResumeTitle = "Original Title",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var command = new UpdateUserCvCommand(cvId, userId, "{\"data\": \"test\"}");

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(403, result.Status);
            Assert.Equal("You can only update your own resume.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsServerError()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "Original Title",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var command = new UpdateUserCvCommand(cvId, userId, "{\"data\": \"test\"}");

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            SetupTransaction();

            _resumeRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserCv>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.Status);
            Assert.StartsWith("Failed to update user CV:", result.Message);
            Assert.Contains("Database connection failed", result.Message);
            Assert.Null(result.ResponseData);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}
