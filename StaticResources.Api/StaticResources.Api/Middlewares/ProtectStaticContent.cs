using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace StaticResources.Api.Middlewares
{
    public static class ProtectStaticContentExtensions
    {
        public static IApplicationBuilder ProtectStaticContent(this WebApplication builder, PathString path)
        {
            return builder.UseMiddleware<ProtectStaticContent>(path);
        }

        public static IApplicationBuilder ProtectStaticContent(this WebApplication builder, PathString path, string policy)
        {
            return builder.UseMiddleware<ProtectStaticContent>(path, policy);
        }
    }

    public class ProtectStaticContent
    {
        private readonly RequestDelegate _next;
        private readonly PathString _path;
        private readonly string _policyName;

        public ProtectStaticContent(RequestDelegate next, PathString path)
        {
            _next = next;
            _path = path;
        }

        public ProtectStaticContent(RequestDelegate next, PathString path, string policyName) : this(next, path)
        {
            _policyName = policyName;
        }


        public async Task Invoke(HttpContext httpContext, IAuthorizationService authorizationService)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_policyName))
                {
                    var authorized = await authorizationService.AuthorizeAsync(httpContext.User, null, _policyName);
                    if (!authorized.Succeeded)
                    {
                        await httpContext.ChallengeAsync();
                        return;
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            await _next(httpContext);
        }
    }
}
