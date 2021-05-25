using System.Threading.Tasks;
using Micro.Auth.Api.Users;
using Micro.Auth.Api.Users.ViewModels;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Users
{
    public class UsersControllerTest
    {
        [Test]
        public async Task TestFindUserByUsername()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.FindByUsername("testUsername")).ReturnsAsync(new User
            {
                Id = "test",
                UserName = "something",
                Email = "email"
            });
            mockUserRepository.Setup(x => x.FindByUsername("testNonExistentUser")).ReturnsAsync(null as User);
            var mockUserService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(mockUserRepository.Object, mockUserService.Object, mockLogger.Object, null);
            Assert.IsFalse((((await controller.FindByUsername("testUsername")).Result as OkObjectResult)?.Value as FindByUsernameResponse)?.Available);
            Assert.IsTrue((((await controller.FindByUsername("testNonExistentUser")).Result as OkObjectResult)?.Value as FindByUsernameResponse)?.Available);
        }

        [Test]
        public async Task TestFindUserByEmail()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.FindByEmail("dev@devcurate.co")).ReturnsAsync(new User
            {
                Id = "test",
                UserName = "something",
                Email = "dev@devcurate.co"
            });
            mockUserRepository.Setup(x => x.FindByUsername("notfound@devcurate.co")).ReturnsAsync(null as User);
            var mockUserService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(mockUserRepository.Object, mockUserService.Object, mockLogger.Object, null);
            Assert.IsFalse((((await controller.FindByEmail("dev@devcurate.co")).Result as OkObjectResult)?.Value as FindByEmailResponse)?.Available);
            Assert.IsTrue((((await controller.FindByEmail("notfound@devcurate.co")).Result as OkObjectResult)?.Value as FindByEmailResponse)?.Available);
        }

        [Test]
        public async Task TestHandlesUserNotFound()
        {
            // var userService = new Mock<IUserService>();
            // userService.Setup(x => x.RequestPasswordReset("test")).ThrowsAsync(new UserNotFoundException());
            // var controller = new UsersController(null, userService.Object, null, null);
            // var response = await controller.RequestPasswordReset(new RequestPasswordReset
            // {
            //     Login = "test"
            // });
            // Assert.IsInstanceOf<NotFoundObjectResult>(response);
            // Assert.AreEqual(StatusCodes.Status404NotFound, (response as NotFoundObjectResult)?.StatusCode);
            // Assert.AreEqual("user not found", ((response as NotFoundObjectResult)?.Value as ProblemDetails)?.Title);
        }

        [Test]
        public async Task TestHandlesEmailError()
        {
            // var userService = new Mock<IUserService>();
            // userService.Setup(x => x.RequestPasswordReset("test")).ThrowsAsync(new SendingEmailFailedException("email sending failed", null));
            // var controller = new UsersController(null, userService.Object, null, null);
            // var response = await controller.RequestPasswordReset(new RequestPasswordReset
            // {
            //     Login = "test"
            // });
            // Assert.IsInstanceOf<ObjectResult>(response);
            // Assert.AreEqual(StatusCodes.Status500InternalServerError, (response as ObjectResult)?.StatusCode);
            // Assert.AreEqual("error handling request", ((response as ObjectResult)?.Value as ProblemDetails)?.Title);
        }

        [Test]
        public async Task TestHappyPath()
        {
            // var userService = new Mock<IUserService>();
            // var controller = new UsersController(null, userService.Object, null, null);
            // var response = await controller.RequestPasswordReset(new RequestPasswordReset
            // {
            //     Login = "test"
            // });
            // Assert.IsInstanceOf<AcceptedResult>(response);
            // Assert.AreEqual(StatusCodes.Status202Accepted, (response as AcceptedResult)?.StatusCode);
        }
    }
}