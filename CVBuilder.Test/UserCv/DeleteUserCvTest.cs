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
    public class DeleteUserCvTest
    {
        private readonly Mock<IGenericRepository<UserCv>> _resumeRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteUserCvCommandHandler _handler;

        public DeleteUserCvTest()
        {
            _resumeRepoMock = new Mock<IGenericRepository<UserCv>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new DeleteUserCvCommandHandler(_resumeRepoMock.Object);
        }

        private void SetupTransaction()
        {
            _resumeRepoMock
                .Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessAndSoftDeletesCv()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "My Resume",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false,
                DeletedAt = null
            };

            var command = new DeleteUserCvCommand(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetByIdAsync(cvId))
                .ReturnsAsync(existingCv);

            SetupTransaction();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV deleted successfully.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.True(result.ResponseData.Success);
            Assert.Equal("Resume deleted successfully.", result.ResponseData.Message);

            _resumeRepoMock.Verify(r => r.GetByIdAsync(cvId), Times.Once);

            _resumeRepoMock.Verify(r => r.UpdateAsync(
                It.Is<UserCv>(cv =>
                    cv.Id == cvId &&
                    cv.IsDeleted == true &&
                    cv.DeletedAt != null)),
                Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyIdHeader_ReturnsBadRequest()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var command = new DeleteUserCvCommand(Guid.Empty, cvId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User ID cannot be empty.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.False(result.ResponseData.Success);

            _resumeRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyId_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCvCommand(userId, Guid.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User CV ID cannot be empty.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.False(result.ResponseData.Success);

            _resumeRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NonExistentCv_ReturnsNotFound()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new DeleteUserCvCommand(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetByIdAsync(cvId))
                .ReturnsAsync((UserCv)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV not found.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.False(result.ResponseData.Success);

            _resumeRepoMock.Verify(r => r.GetByIdAsync(cvId), Times.Once);
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
                ResumeTitle = "My Resume",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var command = new DeleteUserCvCommand(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetByIdAsync(cvId))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(403, result.Status);
            Assert.Equal("You can only delete your own resume.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.False(result.ResponseData.Success);

            _resumeRepoMock.Verify(r => r.GetByIdAsync(cvId), Times.Once);
            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserCv>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AlreadyDeleted_ReturnsBadRequest()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "My Resume",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = true, // Already deleted
                DeletedAt = DateTime.UtcNow.AddDays(-1)
            };

            var command = new DeleteUserCvCommand(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetByIdAsync(cvId))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User CV is already deleted.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.False(result.ResponseData.Success);

            _resumeRepoMock.Verify(r => r.GetByIdAsync(cvId), Times.Once);
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
                ResumeTitle = "My Resume",
                Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var command = new DeleteUserCvCommand(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetByIdAsync(cvId))
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
            Assert.StartsWith("Failed to delete user CV:", result.Message);
            Assert.Contains("Database connection failed", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.False(result.ResponseData.Success);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}
