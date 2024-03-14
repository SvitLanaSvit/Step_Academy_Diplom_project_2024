namespace Diplom_project_2024.Services
{
    public class AccessTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var accessToken = context.Request.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            await _next(context);
        }
    }
}
