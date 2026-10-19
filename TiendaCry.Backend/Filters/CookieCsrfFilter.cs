using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TiendaCry.Backend.Filters
{
    public class CookieCsrfFilter : IAsyncAuthorizationFilter
    {
        private readonly IAntiforgery _antiforgery;

        public CookieCsrfFilter(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method) || HttpMethods.IsOptions(request.Method))
            {
                return;
            }

            if (!request.Cookies.ContainsKey("access_token") && !request.Cookies.ContainsKey("refresh_token"))
            {
                return;
            }

            try
            {
                await _antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                context.Result = new BadRequestObjectResult("El token CSRF no es válido.");
            }
        }
    }
}
