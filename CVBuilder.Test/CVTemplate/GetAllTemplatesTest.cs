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
using static CVBuilder.Contract.UseCases.CvTemplate.Query;
using static CVBuilder.Contract.UseCases.CvTemplate.Request;

namespace CVBuilder.Test
{
    public class GetAllTemplatesTest
    {
        private readonly Mock<ISender> _senderMock;
        private readonly CvTemplatesController _controller;

        public GetAllTemplatesTest()
        {
            _senderMock = new Mock<ISender>();
            _controller = new CvTemplatesController(_senderMock.Object);
        }

        [Fact]
        public async Task GetTemplates_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new GetAllCvTemplatesRequest(
                PageNumber: 1,
                PageSize: 10,
                IncludeDeleted: false
            );

            var templates = new List<CvTemplateDto>
            {
                new CvTemplateDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Template 1",
                    Description = "First template",
                    ThumbnailUrl = "http://example.com/thumb1.png",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new CvTemplateDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Template 2",
                    Description = "Second template",
                    ThumbnailUrl = "http://example.com/thumb2.png",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            var expectedResponse = new BaseResponseDto<IEnumerable<CvTemplateDto>>
            {
                Status = 200,
                Message = "CV templates retrieved successfully.",
                ResponseData = templates
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetAllCvTemplatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplates(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("CV templates retrieved successfully.", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(2, result.ResponseData.Count());
            _senderMock.Verify(s => s.Send(It.Is<GetAllCvTemplatesQuery>(q =>
                q.PageNumber == request.PageNumber &&
                q.PageSize == request.PageSize &&
                q.IncludeDeleted == request.IncludeDeleted), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplates_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            var request = new GetAllCvTemplatesRequest(
                PageNumber: 2,
                PageSize: 5,
                IncludeDeleted: false
            );

            var templates = new List<CvTemplateDto>
            {
                new CvTemplateDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Template Page 2",
                    Description = "Template from page 2",
                    ThumbnailUrl = "http://example.com/thumb.png",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            var expectedResponse = new BaseResponseDto<IEnumerable<CvTemplateDto>>
            {
                Status = 200,
                Message = "CV templates retrieved successfully.",
                ResponseData = templates
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetAllCvTemplatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplates(request);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.NotNull(result.ResponseData);
            Assert.Single(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetAllCvTemplatesQuery>(q =>
                q.PageNumber == 2 && q.PageSize == 5), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplates_IncludeDeleted_ReturnsAllTemplates()
        {
            // Arrange
            var request = new GetAllCvTemplatesRequest(
                PageNumber: 1,
                PageSize: 10,
                IncludeDeleted: true
            );

            var templates = new List<CvTemplateDto>
            {
                new CvTemplateDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Template",
                    Description = "Active template",
                    ThumbnailUrl = "http://example.com/thumb1.png",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new CvTemplateDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Deleted Template",
                    Description = "Deleted template",
                    ThumbnailUrl = "http://example.com/thumb2.png",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = true
                }
            };

            var expectedResponse = new BaseResponseDto<IEnumerable<CvTemplateDto>>
            {
                Status = 200,
                Message = "CV templates retrieved successfully.",
                ResponseData = templates
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetAllCvTemplatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplates(request);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(2, result.ResponseData.Count());
            Assert.Contains(result.ResponseData, t => t.IsDeleted == true);
            Assert.Contains(result.ResponseData, t => t.IsDeleted == false);
            _senderMock.Verify(s => s.Send(It.Is<GetAllCvTemplatesQuery>(q =>
                q.IncludeDeleted == true), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplates_InvalidPageNumber_ReturnsBadRequest()
        {
            // Arrange
            var request = new GetAllCvTemplatesRequest(
                PageNumber: 0,
                PageSize: 10,
                IncludeDeleted: false
            );

            var expectedResponse = new BaseResponseDto<IEnumerable<CvTemplateDto>>
            {
                Status = 400,
                Message = "Page number and page size must be positive.",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetAllCvTemplatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplates(request);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Page number and page size must be positive.", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.Is<GetAllCvTemplatesQuery>(q =>
                q.PageNumber == 0), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetTemplates_Exception_ReturnsErrorStatus()
        {
            // Arrange
            var request = new GetAllCvTemplatesRequest(
                PageNumber: 1,
                PageSize: 10,
                IncludeDeleted: false
            );

            var expectedResponse = new BaseResponseDto<IEnumerable<CvTemplateDto>>
            {
                Status = 500,
                Message = "Failed to retrieve CV templates: Database connection error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetAllCvTemplatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTemplates(request);

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Failed to retrieve CV templates", result.Message);
            Assert.Null(result.ResponseData);
            _senderMock.Verify(s => s.Send(It.IsAny<GetAllCvTemplatesQuery>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
