using System.Collections.Generic;
using System.Security.Claims;
using Micro.Auth.Api.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Authentication
{
    public class RequirePermissionTest
    {
        [Test]
        public void TestRequirePermissionForbidsResultIfNoUser()
        {
            var requirePermission = new RequirePermission("test-permission");
            var ctx = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
            requirePermission.OnAuthorization(ctx);
            Assert.IsInstanceOf<ForbidResult>(ctx.Result);
        }

        [Test]
        public void TestRequirePermissionForbidsResultIfUserHasNoPermission()
        {
            var requirePermission = new RequirePermission("test-permission");
            var ctx = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal
                {
                    Claims = { }
                }
            }, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
            requirePermission.OnAuthorization(ctx);
            Assert.IsInstanceOf<ForbidResult>(ctx.Result);
        }

        [Test]
        public void TestRequirePermissionOkWhenUserHasPermission()
        {
            var requirePermission = new RequirePermission("test-permission");
            var claims = new []
            {
                new Claim("name", "Alice"),
                new Claim("role", "sudo"),
                new Claim("Permission", "test-permission"),
                new Claim("Permission", "test-permission1"),
                new Claim("Permission", "test-permission2"),
            };
            var claimsPrincipal = new Mock<ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Claims).Returns(claims);

            var ctx = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = claimsPrincipal.Object
            }, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
            requirePermission.OnAuthorization(ctx);
            Assert.IsNull(ctx.Result);
        }

        [Test]
        public void TestRequirePermissionOkWhenUserHasSudoPermission()
        {
            var claims = new []
            {
                new Claim("name", "Alice"),
                new Claim("role", "sudo"),
                new Claim("Permission", "sudo")
            };
            var claimsPrincipal = new Mock<ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Claims).Returns(claims);

            var ctx = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = claimsPrincipal.Object
            }, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
            var requirePermission = new RequirePermission("test-permission");
            requirePermission.OnAuthorization(ctx);
            Assert.IsNull(ctx.Result);
            requirePermission = new RequirePermission("another-permission");
            requirePermission.OnAuthorization(ctx);
            Assert.IsNull(ctx.Result);
        }
    }
}
