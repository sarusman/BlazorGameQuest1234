using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlazorGame.GameService.Filters
{
    public sealed class RequireLoginAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cookies = context.HttpContext.Request.Cookies;
            var hasPseudo = cookies.TryGetValue("pseudo", out var pseudo) && !string.IsNullOrWhiteSpace(pseudo);
            var hasJoueurId = cookies.TryGetValue("joueurId", out var joueurId) && Guid.TryParse(joueurId, out _);

            if (!hasPseudo || !hasJoueurId)
            {
                context.Result = new RedirectResult("/?msg=login");
                return;
            }

            await next();
        }
    }
}