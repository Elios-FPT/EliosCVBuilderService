using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CVBuilder.Contract.Shared;
using CVBuilder.Core.Handler.UserCv.Query;
using CVBuilder.Core.Interfaces;
using Elios.CVBuilder.Domain.Models;
using Moq;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Test
{
    public class GetUserCvByIdTest
    {
        private readonly Mock<IGenericRepository<UserCv>> _resumeRepoMock;
        private readonly GetUserCvByIdQueryHandler _handler;

        public GetUserCvByIdTest()
        {
            _resumeRepoMock = new Mock<IGenericRepository<UserCv>>();
            _handler = new GetUserCvByIdQueryHandler(_resumeRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessWithJsonData()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var jsonString = """
            {
                "personalInfo": {
                    "firstName": "John",
                    "lastName": "Doe",
                    "email": "john@example.com"
                }
            }
            """;

            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "My Resume",
                Data = jsonString,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var query = new GetUserCvByIdQuery(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV retrieved successfully.", result.Message);

            var jsonElement = result.ResponseData;
            Assert.True(jsonElement.TryGetProperty("personalInfo", out var personalInfo));
            Assert.Equal("John", personalInfo.GetProperty("firstName").GetString());
            Assert.Equal("Doe", personalInfo.GetProperty("lastName").GetString());
            Assert.Equal("john@example.com", personalInfo.GetProperty("email").GetString());

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyIdHeader_ReturnsBadRequest()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var query = new GetUserCvByIdQuery(Guid.Empty, cvId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User ID cannot be empty.", result.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyId_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserCvByIdQuery(userId, Guid.Empty);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User CV ID cannot be empty.", result.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Never);
        }

        [Fact]
        public async Task Handle_NonExistentCv_ReturnsNotFound()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetUserCvByIdQuery(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync((UserCv)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV not found.", result.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbidden()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var jsonString = """
            {
                "personalInfo": {
                    "firstName": "John"
                }
            }
            """;

            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = differentUserId, // Different owner
                ResumeTitle = "My Resume",
                Data = jsonString,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var query = new GetUserCvByIdQuery(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(403, result.Status);
            Assert.Equal("You can only view your own resume.", result.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyData_ReturnsNotFound()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "My Resume",
                Data = "", // Empty data
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var query = new GetUserCvByIdQuery(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV body is empty.", result.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsServerError()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetUserCvByIdQuery(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.Status);
            Assert.StartsWith("Failed to retrieve user CV:", result.Message);
            Assert.Contains("Database connection failed", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidJsonData_ReturnsServerError()
        {
            // Arrange
            var cvId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existingCv = new UserCv
            {
                Id = cvId,
                OwnerId = userId,
                ResumeTitle = "My Resume",
                Data = "invalid json {{{", // Invalid JSON
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var query = new GetUserCvByIdQuery(userId, cvId);

            _resumeRepoMock
                .Setup(r => r.GetOneAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(existingCv);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.Status);
            Assert.StartsWith("Failed to retrieve user CV:", result.Message);

            _resumeRepoMock.Verify(r => r.GetOneAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                null,
                null), Times.Once);
        }
    }
}
