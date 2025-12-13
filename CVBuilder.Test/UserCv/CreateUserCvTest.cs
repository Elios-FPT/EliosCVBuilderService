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
    public class CreateUserCvHandlerTest
    {
        private readonly Mock<IGenericRepository<UserCv>> _resumeRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateUserCvCommandHandler _handler;

        public CreateUserCvHandlerTest()
        {
            _resumeRepoMock = new Mock<IGenericRepository<UserCv>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreateUserCvCommandHandler(_resumeRepoMock.Object);
        }

        private void SetupTransaction()
        {
            _resumeRepoMock
                .Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsCreatedAndSavesCv()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var command = new CreateUserCvCommand(ownerId, "My Resume");

            SetupTransaction();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.Status);
            Assert.Equal("User CV created successfully.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.NotEqual(Guid.Empty, result.ResponseData.Id);

            _resumeRepoMock.Verify(r => r.AddAsync(
                It.Is<UserCv>(cv =>
                    cv.OwnerId == ownerId &&
                    cv.ResumeTitle == "My Resume" &&
                    !string.IsNullOrWhiteSpace(cv.Data) &&
                    cv.CreatedAt != default &&
                    cv.UpdatedAt != default &&
                    cv.IsDeleted == false)),
                Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyOwnerId_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateUserCvCommand(Guid.Empty, "My Resume");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("Owner ID is required.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.AddAsync(It.IsAny<UserCv>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_InvalidResumeTitle_ReturnsBadRequest(string resumeTitle)
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var command = new CreateUserCvCommand(ownerId, resumeTitle);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("Resume title is required.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.BeginTransactionAsync(), Times.Never);
            _resumeRepoMock.Verify(r => r.AddAsync(It.IsAny<UserCv>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsServerError()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var command = new CreateUserCvCommand(ownerId, "My Resume");

            SetupTransaction();

            _resumeRepoMock
                .Setup(r => r.AddAsync(It.IsAny<UserCv>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.Status);
            Assert.StartsWith("Failed to create user CV:", result.Message);
            Assert.Contains("Database connection failed", result.Message);
            Assert.Null(result.ResponseData);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}
