using System;
using System.Threading.Tasks;
using App.Metrics;
using Micro.Auth.Api.Authentication;
using Micro.Auth.Api.Authentication.ViewModels;
using Micro.Auth.Api.Users;
using Micro.Auth.Api.Users.ViewModels;
using Micro.Auth.Business.RefreshTokens;
using Micro.Auth.Business.Users;
using Micro.Auth.Storage.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Micro.Auth.UnitTest.Authentication
{
    public class SessionControllerTest
    {
        [Test]
        public async Task TestLoginReturnsBadRequestForNoWrongAuthData()
        {
            var mockLogger = new Mock<ILogger<SessionController>>();
            var controller = new SessionController(mockLogger.Object, null, null, null);
            var response = await controller.New("Bearer randomtoken");
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task TestLoginReturnsServerErrorIfUserServerThrows()
        {
            var mockLogger = new Mock<ILogger<SessionController>>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.Login(It.IsAny<LoginRequest>())).ThrowsAsync(new Exception());
            var controller = new SessionController(mockLogger.Object, null, null, null);
            var response = await controller.New("Bearer bmlzaDpwYXNz");
            Assert.IsInstanceOf<ObjectResult>(response);
            Assert.AreEqual(500, (response as ObjectResult)?.StatusCode);
        }

        [Test]
        public async Task TestLoginReturnsUnauthorizedForUnsucessfulLogins()
        {
            var mockLogger = new Mock<ILogger<SessionController>>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.Login(It.IsAny<LoginRequest>())).ReturnsAsync((SignInResult.Failed, null));
            var controller = new SessionController(mockLogger.Object, mockUserService.Object, null, null);
            var response = await controller.New("Bearer bmlzaDpwYXNz");
            Assert.IsInstanceOf<UnauthorizedObjectResult>(response);
        }

        [Test]
        public async Task TestLoginReturnsOkResult()
        {
            var mockLogger = new Mock<ILogger<SessionController>>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.Login(It.IsAny<LoginRequest>())).ReturnsAsync((SignInResult.Success, new LoginSuccessResponse
            {
                Jwt = "testJwtToken",
                RefreshToken = "testRefreshToken"
            }));
            var controller = new SessionController(mockLogger.Object, mockUserService.Object, null, null);
            var response = await controller.New("Bearer bmlzaDpwYXNz");
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual("testJwtToken", ((response as OkObjectResult)?.Value as LoginSuccessResponse)?.Jwt);
            Assert.AreEqual("testRefreshToken", ((response as OkObjectResult)?.Value as LoginSuccessResponse)?.RefreshToken);
        }

        [Test]
        public async Task TestRefreshReturnsNewJwtIfCorrect()
        {
            var refreshTokenService = new Mock<IRefreshTokenService>();
            refreshTokenService.Setup(x => x.Refresh("testRefreshToken")).ReturnsAsync("sampleJwt");
            var controller = new SessionController(null, null, null, refreshTokenService.Object);
            var response = await controller.Refresh("Bearer testRefreshToken");
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<RefreshTokenSuccessResponse>((response as OkObjectResult)?.Value);
            var successResponse = (response as OkObjectResult)?.Value as RefreshTokenSuccessResponse;
            Assert.AreEqual("sampleJwt", successResponse?.Jwt);
        }

        [Test]
        public async Task TestRefreshReturnsNotFoundWhenNotFound()
        {
            var refreshTokenService = new Mock<IRefreshTokenService>();
            refreshTokenService.Setup(x => x.Refresh("testRefreshToken")).ThrowsAsync(new RefreshTokenNotFoundException());
            var controller = new SessionController(null, null, null, refreshTokenService.Object);
            var response = await controller.Refresh("Bearer testRefreshToken");
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }

        [Test]
        public async Task TestRefreshReturnsServerErrorWhenOtherExceptionsAreThrown()
        {
            var refreshTokenService = new Mock<IRefreshTokenService>();
            var mockLogger = new Mock<ILogger<SessionController>>();
            refreshTokenService.Setup(x => x.Refresh("testRefreshToken")).ThrowsAsync(new Exception());
            var controller = new SessionController(mockLogger.Object, null, null, refreshTokenService.Object);
            var response = await controller.Refresh("Bearer testRefreshToken");
            Assert.IsInstanceOf<ObjectResult>(response);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (response as ObjectResult)?.StatusCode);
        }
    }
}