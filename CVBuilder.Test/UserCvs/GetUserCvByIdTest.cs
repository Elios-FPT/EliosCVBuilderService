using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Web.Controllers;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Test
{
    public class GetUserCvByIdTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public GetUserCvByIdTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new UserCvsController(_senderMock.Object);
        }

        [Fact]
        public async Task GetUserCv_ValidId_ReturnsSuccess()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 200,
                Message = "User CV retrieved successfully",
                ResponseData = new UserCvDto
                {
                    Id = userCvId,
                    UserId = Guid.NewGuid(),
                    TemplateId = Guid.NewGuid(),
                    Title = "My CV",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Template = new CvTemplateDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Template",
                        Description = "A sample template",
                        ThumbnailUrl = "http://example.com/thumb.png",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                }
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(userCvId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV retrieved successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(userCvId, result.ResponseData.Id);
            Assert.Equal("My CV", result.ResponseData.Title);
            Assert.NotNull(result.ResponseData.Template);
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvByIdQuery>(q =>
                q.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserCv_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 404,
                Message = "User CV not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(userCvId);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV not found", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvByIdQuery>(q =>
                q.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserCv_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 500,
                Message = "Failed to retrieve user CV: Database connection error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCv(userCvId);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to retrieve user CV", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetUserCvByIdQuery>(q =>
                q.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
