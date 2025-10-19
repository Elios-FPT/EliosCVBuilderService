using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Web.Controllers;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.CvTemplate.Query;

namespace CVBuilder.Test
{
    public class GetTemplateByIdTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly CvTemplatesController _controller;

        public GetTemplateByIdTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new CvTemplatesController(_senderMock.Object);
        }

        [Fact]
        public async Task GetTemplate_ValidId_ReturnsSuccess()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 200,
                Message = "Template retrieved successfully",
                ResponseData = new CvTemplateDto
                {
                    Id = templateId,
                    Name = "Test Template",
                    Description = "A sample CV template",
                    ThumbnailUrl = "http://example.com/thumbnail.png",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetCvTemplateByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplate(templateId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("Template retrieved successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(templateId, result.ResponseData.Id);
            Assert.Equal("Test Template", result.ResponseData.Name);
            _senderMock.Verify(s => s.Send(It.Is<GetCvTemplateByIdQuery>(q =>
                q.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplate_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 404,
                Message = "Template not found",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetCvTemplateByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplate(templateId);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("Template not found", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetCvTemplateByIdQuery>(q =>
                q.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplate_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var templateId = Guid.Empty;
            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 400,
                Message = "Invalid template ID",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetCvTemplateByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplate(templateId);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Invalid template ID", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetCvTemplateByIdQuery>(q =>
                q.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplate_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<CvTemplateDto>
            {
                Status = 500,
                Message = "Internal server error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetCvTemplateByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplate(templateId);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Internal server error", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetCvTemplateByIdQuery>(q =>
                q.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
