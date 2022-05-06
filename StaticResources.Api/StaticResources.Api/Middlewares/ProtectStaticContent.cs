using System.Net;

namespace StaticResources.Api.Middlewares
{
    public static class ProtectStaticContentExtensions
    {
        public static IApplicationBuilder ProtectStaticContent(this WebApplication builder, PathString path)
        {
            return builder.UseMiddleware<ProtectStaticContent>(path);
        }
    }

    public class ProtectStaticContent
    {
        private readonly RequestDelegate _next;
        private readonly PathString _path;

        public ProtectStaticContent(RequestDelegate next, PathString path)
        {
            _next = next;
            _path = path;
        }


        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path, StringComparison.CurrentCultureIgnoreCase))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
            await _next(httpContext);
        }
    }
}
