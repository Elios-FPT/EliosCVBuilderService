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
    public class CreateUserCvTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly UserCvsController _controller;

        public CreateUserCvTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new UserCvsController(_senderMock.Object);
        }

        [Fact]
        public async Task CreateUserCv_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new CreateUserCvRequest(
                UserId: Guid.NewGuid(),
                TemplateId: Guid.NewGuid(),
                Title: "My CV"
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 200,
                Message = "User CV created successfully",
                ResponseData = new UserCvDto
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    TemplateId = request.TemplateId,
                    Title = request.Title,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Template = new CvTemplateDto
                    {
                        Id = request.TemplateId,
                        Name = "Test Template",
                        Description = "A sample template",
                        ThumbnailUrl = "http://example.com/thumb.png",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                }
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("User CV created successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(request.UserId, result.ResponseData.UserId);
            Assert.Equal(request.TemplateId, result.ResponseData.TemplateId);
            Assert.Equal(request.Title, result.ResponseData.Title);
            Assert.NotNull(result.ResponseData.Template);
            _senderMock.Verify(s => s.Send(It.Is<CreateUserCvCommand>(cmd =>
                cmd.UserId == request.UserId &&
                cmd.TemplateId == request.TemplateId &&
                cmd.Title == request.Title), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CreateUserCv_NullTitle_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateUserCvRequest(
                UserId: Guid.NewGuid(),
                TemplateId: Guid.NewGuid(),
                Title: null // ResumeTitle is required, should cause validation error
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 400,
                Message = "ResumeTitle cannot be null or empty.",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("ResumeTitle cannot be null or empty.", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<CreateUserCvCommand>(cmd =>
                cmd.Title == null), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CreateUserCv_InvalidTemplateId_ReturnsNotFound()
        {
            // Arrange
            var request = new CreateUserCvRequest(
                UserId: Guid.NewGuid(),
                TemplateId: Guid.NewGuid(),
                Title: "My CV"
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 404,
                Message = "Template not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("Template not found", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<CreateUserCvCommand>(cmd =>
                cmd.TemplateId == request.TemplateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CreateUserCv_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var request = new CreateUserCvRequest(
                UserId: Guid.NewGuid(),
                TemplateId: Guid.NewGuid(),
                Title: "My CV"
            );

            var expectedResponse = new BaseResponseDto<UserCvDto>
            {
                Status = 500,
                Message = "Failed to create user CV: Database connection error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateUserCv(request);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to create user CV", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.IsAny<CreateUserCvCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
