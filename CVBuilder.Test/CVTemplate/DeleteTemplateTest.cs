using CVBuilder.Contract.Shared;
using CVBuilder.Web.Controllers;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.CvTemplate.Command;

namespace CVBuilder.Test
{
    public class DeleteTemplateTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly CvTemplatesController _controller;

        public DeleteTemplateTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new CvTemplatesController(_senderMock.Object);
        }

        [Fact]
        public async Task DeleteTemplate_ValidId_ReturnsSuccess()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<bool>
            {
                Status = 200,
                Message = "Template deleted successfully",
                ResponseData = true
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteTemplate(templateId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("Template deleted successfully", result.Message);
            Assert.True(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<DeleteCvTemplateCommand>(cmd =>
                cmd.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteTemplate_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<bool>
            {
                Status = 404,
                Message = "Template not found",
                ResponseData = false
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteTemplate(templateId);

            // Assert
            Assert.Equal(404, result.Status);
            Assert.Equal("Template not found", result.Message);
            Assert.False(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<DeleteCvTemplateCommand>(cmd =>
                cmd.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteTemplate_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var expectedResponse = new BaseResponseDto<bool>
            {
                Status = 500,
                Message = "Failed to delete CV template: Database connection error",
                ResponseData = false
            };

            _senderMock.Setup(s => s.Send(It.IsAny<DeleteCvTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteTemplate(templateId);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to delete CV template", result.Message);
            Assert.False(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<DeleteCvTemplateCommand>(cmd =>
                cmd.Id == templateId), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
