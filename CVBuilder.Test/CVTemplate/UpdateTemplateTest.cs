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
using static CVBuilder.Contract.UseCases.CvTemplate.Command;
using static CVBuilder.Contract.UseCases.CvTemplate.Request;

namespace CVBuilder.Test
{
    public class UpdateTemplateTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly CvTemplatesController _controller;

        public UpdateTemplateTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new CvTemplatesController(_senderMock.Object);
        }

        [Fact]
        public async Task UpdateTemplate_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var request = new UpdateCvTemplateRequest(
                Name: "Updated Template",
                Description: "Updated description",
                ThumbnailUrl: "http://example.com/updated-thumbnail.png"
            );

            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 200,
                Message = "Template updated successfully",
                ResponseData = new CvTemplateDto
                {
                    Id = templateId,
                    Name = request.Name,
                    Description = request.Description,
                    ThumbnailUrl = request.ThumbnailUrl,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false
                }
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateTemplate(templateId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("Template updated successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(templateId, result.ResponseData.Id);
            Assert.Equal(request.Name, result.ResponseData.Name);
            Assert.Equal(request.Description, result.ResponseData.Description);
            Assert.Equal(request.ThumbnailUrl, result.ResponseData.ThumbnailUrl);
            _senderMock.Verify(s => s.Send(It.Is<UpdateCvTemplateCommand>(cmd =>
                cmd.Id == templateId &&
                cmd.Name == request.Name &&
                cmd.Description == request.Description &&
                cmd.ThumbnailUrl == request.ThumbnailUrl), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateTemplate_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var request = new UpdateCvTemplateRequest(
                Name: "Updated Template",
                Description: "Updated description",
                ThumbnailUrl: "http://example.com/updated-thumbnail.png"
            );

            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 404,
                Message = "Template not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateTemplate(templateId, request);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("Template not found", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<UpdateCvTemplateCommand>(cmd =>
                cmd.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateTemplate_NullName_ReturnsBadRequest()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var request = new UpdateCvTemplateRequest(
                Name: null, // Name is required, should cause validation error
                Description: "Updated description",
                ThumbnailUrl: "http://example.com/updated-thumbnail.png"
            );

            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 400,
                Message = "Template name cannot be null or empty.",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateTemplate(templateId, request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Template name cannot be null or empty.", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<UpdateCvTemplateCommand>(cmd =>
                cmd.Id == templateId && cmd.Name == null), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateTemplate_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var request = new UpdateCvTemplateRequest(
                Name: "Updated Template",
                Description: "Updated description",
                ThumbnailUrl: "http://example.com/updated-thumbnail.png"
            );

            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 500,
                Message = "Failed to update CV template: Database connection error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<UpdateCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateTemplate(templateId, request);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to update CV template", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<UpdateCvTemplateCommand>(cmd =>
                cmd.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
