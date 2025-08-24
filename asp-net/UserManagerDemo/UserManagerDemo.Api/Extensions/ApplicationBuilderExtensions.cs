using UserManagerDemo.Api.MiddleWares;

namespace UserManagerDemo.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UnitOfWorkMiddleware>();
    }
}