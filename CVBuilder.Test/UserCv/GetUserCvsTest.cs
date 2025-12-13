using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Handler.UserCv.Query;
using CVBuilder.Core.Interfaces;
using Elios.CVBuilder.Domain.Models;
using Moq;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Test
{
    public class GetUserCvsTest
    {
        private readonly Mock<IGenericRepository<UserCv>> _resumeRepoMock;
        private readonly GetUserCvsQueryHandler _handler;

        public GetUserCvsTest()
        {
            _resumeRepoMock = new Mock<IGenericRepository<UserCv>>();
            _handler = new GetUserCvsQueryHandler(_resumeRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessWithCvList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserCvsQuery(userId, 1, 20);

            var userCvs = new List<UserCv>
            {
                new UserCv
                {
                    Id = Guid.NewGuid(),
                    OwnerId = userId,
                    ResumeTitle = "Software Engineer Resume",
                    Data = "{\"personalInfo\": {\"firstName\": \"John\"}}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new UserCv
                {
                    Id = Guid.NewGuid(),
                    OwnerId = userId,
                    ResumeTitle = "Product Manager Resume",
                    Data = "{\"personalInfo\": {\"firstName\": \"Jane\"}}",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false
                }
            };

            _resumeRepoMock
                .Setup(r => r.GetListAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                    null,
                    20,
                    1))
                .ReturnsAsync(userCvs);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CVs retrieved successfully.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(2, result.ResponseData.Count());

            var summaries = result.ResponseData.ToList();
            Assert.Equal("Software Engineer Resume", summaries[0].ResumeTitle);
            Assert.Equal("Product Manager Resume", summaries[1].ResumeTitle);

            _resumeRepoMock.Verify(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                null,
                20,
                1), Times.Once);
        }

        [Fact]
        public async Task Handle_NoCvsFound_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserCvsQuery(userId, 1, 20);

            _resumeRepoMock
                .Setup(r => r.GetListAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                    null,
                    20,
                    1))
                .ReturnsAsync(new List<UserCv>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("No user CVs found.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Empty(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                null,
                20,
                1), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyUserId_ReturnsBadRequest()
        {
            // Arrange
            var query = new GetUserCvsQuery(Guid.Empty, 1, 20);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("User ID cannot be empty.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                null,
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(0, 20)]
        [InlineData(-1, 20)]
        [InlineData(1, 0)]
        [InlineData(1, -5)]
        public async Task Handle_InvalidPagination_ReturnsBadRequest(int pageNumber, int pageSize)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserCvsQuery(userId, pageNumber, pageSize);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.Status);
            Assert.Equal("Page number and page size must be positive.", result.Message);
            Assert.Null(result.ResponseData);

            _resumeRepoMock.Verify(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                null,
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserCvsQuery(userId, 1, 20);

            _resumeRepoMock
                .Setup(r => r.GetListAsync(
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<UserCv, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<System.Func<IQueryable<UserCv>, IOrderedQueryable<UserCv>>>>(),
                    null,
                    20,
                    1))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.Status);
            Assert.StartsWith("Failed to retrieve user CVs:", result.Message);
            Assert.Contains("Database connection failed", result.Message);
            Assert.Null(result.ResponseData);
        }
    }
}
