using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CVBuilder.Contract.UseCases.UserCv.Query;

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

        private void SetupHttpContext(string userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Auth-Request-User"] = userId;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetUserCvs_ValidHeader_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var cvSummaries = new List<UserCvSummaryDto>
            {
                new UserCvSummaryDto(Guid.NewGuid(), "Software Engineer Resume", DateTime.UtcNow),
                new UserCvSummaryDto(Guid.NewGuid(), "Product Manager Resume", DateTime.UtcNow.AddDays(-1))
            };

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvSummaryDto>>
            {
                Status = 200,
                Message = "CVs retrieved successfully",
                ResponseData = cvSummaries
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.Status);
            Assert.Equal("CVs retrieved successfully", result.Message);
            Assert.NotNull(result.ResponseData);
            Assert.Equal(2, result.ResponseData.Count());

            _senderMock.Verify(s => s.Send(
                It.Is<GetUserCvsQuery>(q =>
                    q.UserId == userId &&
                    q.PageNumber == 1 &&
                    q.PageSize == 20
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Fact]
        public async Task GetUserCvs_MissingHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _controller.GetUserCvs()
            );
        }

        [Fact]
        public async Task GetUserCvs_InvalidHeader_ThrowsException()
        {
            // Arrange
            SetupHttpContext("invalid");

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(
                async () => await _controller.GetUserCvs()
            );
        }

        [Fact]
        public async Task GetUserCvs_HandlerException_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupHttpContext(userId.ToString());

            var expectedResponse = new BaseResponseDto<IEnumerable<UserCvSummaryDto>>
            {
                Status = 500,
                Message = "Internal server error",
                ResponseData = null
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetUserCvsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUserCvs();

            // Assert
            Assert.Equal(500, result.Status);
            Assert.Contains("Internal server error", result.Message);
            Assert.Null(result.ResponseData);

            _senderMock.Verify(s => s.Send(
                It.IsAny<GetUserCvsQuery>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }
    }
}
