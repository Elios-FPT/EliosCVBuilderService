using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Web.Controllers;
using MediatR;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Command;
using static CVBuilder.Contract.UseCases.UserCv.Request;

namespace CVBuilder.Test
{
    public class UpdateUserCvTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public UpdateUserCvTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new UserCvsController(_senderMock.Object);
        }

        [Fact]
        public async Task UpdateUserCv_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var request = new UpdateUserCvRequest(
                Title: "Updated CV ResumeTitle"
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 200,
                Message = "User CV updated successfully",
                ResponseData = new UserCvDto
                {
                    Id = userCvId,
                    UserId = Guid.NewGuid(),
                    TemplateId = Guid.NewGuid(),
                    Title = request.Title,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow,
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

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(userCvId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV updated successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(userCvId, result.ResponseData.Id);
            Assert.Equal(request.Title, result.ResponseData.Title);
            Assert.NotNull(result.ResponseData.UpdatedAt);
            _senderMock.Verify(s => s.Send(It.Is<UpdateUserCvCommand>(cmd =>
                cmd.Id == userCvId &&
                cmd.Title == request.Title), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateUserCv_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var request = new UpdateUserCvRequest(
                Title: "Updated CV ResumeTitle"
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 404,
                Message = "User CV not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(userCvId, request);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("User CV not found", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<UpdateUserCvCommand>(cmd =>
                cmd.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateUserCv_NullTitle_ReturnsBadRequest()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var request = new UpdateUserCvRequest(
                Title: null // ResumeTitle is required, should cause validation error
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 400,
                Message = "ResumeTitle cannot be null or empty.",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(userCvId, request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("ResumeTitle cannot be null or empty.", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<UpdateUserCvCommand>(cmd =>
                cmd.Id == userCvId && cmd.Title == null), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateUserCv_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var userCvId = Guid.NewGuid();
            var request = new UpdateUserCvRequest(
                Title: "Updated CV ResumeTitle"
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 500,
                Message = "Failed to update user CV: Database connection error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateUserCv(userCvId, request);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to update user CV", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<UpdateUserCvCommand>(cmd =>
                cmd.Id == userCvId), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
