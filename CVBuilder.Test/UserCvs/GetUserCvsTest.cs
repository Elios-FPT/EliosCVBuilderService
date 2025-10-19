using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Web.Controllers;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Query;
using static CVBuilder.Contract.UseCases.UserCv.Request;

namespace CVBuilder.Test
{
    public class GetUserCvsTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public GetUserCvsTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new UserCvsController(_senderMock.Object);
        }

        [Fact]
        public async Task GetUserCvs_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserCvsRequest(
                UserId: userId,
                PageNumber: 1,
                PageSize: 10
            );

            var userCvs = new List<UserCvDto>
            {
                new UserCvDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TemplateId = Guid.NewGuid(),
                    Title = "CV 1",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Template = new CvTemplateDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Template 1",
                        Description = "First template",
                        ThumbnailUrl = "http://example.com/thumb1.png",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                },
                new UserCvDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TemplateId = Guid.NewGuid(),
                    Title = "CV 2",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Template = new CvTemplateDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Template 2",
                        Description = "Second template",
                        ThumbnailUrl = "http://example.com/thumb2.png",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                }
            };

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvDto>>
            {
                Status = 200,
                Message = "User CVs retrieved successfully.",
                ResponseData = userCvs
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CVs retrieved successfully.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(2, result.ResponseData.Count());
            Assert.All(result.ResponseData, cv => Assert.Equal(userId, cv.UserId));
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvsQuery>(q =>
                q.UserId == request.UserId &&
                q.PageNumber == request.PageNumber &&
                q.PageSize == request.PageSize), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserCvs_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserCvsRequest(
                UserId: userId,
                PageNumber: 2,
                PageSize: 5
            );

            var userCvs = new List<UserCvDto>
            {
                new UserCvDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TemplateId = Guid.NewGuid(),
                    Title = "CV Page 2",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Template = new CvTemplateDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Template",
                        Description = "Template description",
                        ThumbnailUrl = "http://example.com/thumb.png",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                }
            };

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvDto>>
            {
                Status = 200,
                Message = "User CVs retrieved successfully.",
                ResponseData = userCvs
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs(request);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.NotNull(result.ResponseData);
            Assert.Single(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvsQuery>(q =>
                q.UserId == userId && q.PageNumber == 2 && q.PageSize == 5), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserCvs_NoResults_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserCvsRequest(
                UserId: userId,
                PageNumber: 1,
                PageSize: 10
            );

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvDto>>
            {
                Status = 200,
                Message = "No user CVs found.",
                ResponseData = new List<UserCvDto>()
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs(request);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.Equal("No user CVs found.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Empty(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvsQuery>(q =>
                q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserCvs_InvalidPageNumber_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserCvsRequest(
                UserId: userId,
                PageNumber: 0,
                PageSize: 10
            );

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvDto>>
            {
                Status = 400,
                Message = "Page number and page size must be positive.",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Page number and page size must be positive.", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvsQuery>(q =>
                q.UserId == userId && q.PageNumber == 0), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserCvs_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserCvsRequest(
                UserId: userId,
                PageNumber: 1,
                PageSize: 10
            );

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvDto>>
            {
                Status = 500,
                Message = "Failed to retrieve user CVs: Database connection error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs(request);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to retrieve user CVs", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
