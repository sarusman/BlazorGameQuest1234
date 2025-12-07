using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using BlazorGame.GameService.Filters;

namespace BlazorGame.Tests.GameServiceTests.ControllersTests
{
    public class RequireLoginAttributeTests
    {
        [Fact]
        public async Task OnActionExecutionAsync_AllCookiesPresent_AllowsAction()
        {
            // Arrange
            var attr = new RequireLoginAttribute();
            var cookies = new Mock<IRequestCookieCollection>();
            cookies.Setup(c => c.TryGetValue("pseudo", out It.Ref<string>.IsAny)).Returns((string key, out string value) => { value = "admin"; return true; });
            cookies.Setup(c => c.TryGetValue("joueurId", out It.Ref<string>.IsAny)).Returns((string key, out string value) => { value = Guid.NewGuid().ToString(); return true; });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = cookies.Object;
            var context = new ActionExecutingContext(
                new ActionContext {
                    HttpContext = httpContext,
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                },
                new System.Collections.Generic.List<IFilterMetadata>(),
                new System.Collections.Generic.Dictionary<string, object?>(),
                new object()
            );
            var executed = false;
            var next = new ActionExecutionDelegate(() => { executed = true; return Task.FromResult<ActionExecutedContext>(default!); });

            // Act
            await attr.OnActionExecutionAsync(context, next);

            // Assert
            Assert.Null(context.Result);
            Assert.True(executed);
        }

        [Fact]
        public async Task OnActionExecutionAsync_MissingCookies_RedirectsToLogin()
        {
            // Arrange
            var attr = new RequireLoginAttribute();
            var cookies = new Mock<IRequestCookieCollection>();
            string? pseudoOut;
            string? joueurIdOut;
            cookies.Setup(c => c.TryGetValue("pseudo", out pseudoOut!)).Returns(false);
            cookies.Setup(c => c.TryGetValue("joueurId", out joueurIdOut!)).Returns(false);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = cookies.Object;
            var context = new ActionExecutingContext(
                new ActionContext {
                    HttpContext = httpContext,
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                },
                new System.Collections.Generic.List<IFilterMetadata>(),
                new System.Collections.Generic.Dictionary<string, object?>(),
                new object()
            );
            var next = new ActionExecutionDelegate(() => Task.FromResult<ActionExecutedContext>(default!));

            // Act
            await attr.OnActionExecutionAsync(context, next);

            // Assert
            Assert.IsType<RedirectResult>(context.Result);
        }
    }
}
