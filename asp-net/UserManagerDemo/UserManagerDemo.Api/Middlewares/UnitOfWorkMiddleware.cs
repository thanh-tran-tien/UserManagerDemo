using UserManagerDemo.Api.Attributes;
using UserManagerDemo.Infrastructure.Persistence;

namespace UserManagerDemo.Api.MiddleWares;

public class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var endpoint = context.GetEndpoint();
        var uowAttr = endpoint?.Metadata.GetMetadata<UnitOfWorkAttribute>();

        if (uowAttr?.Enabled != true)
        {
            await _next(context);
            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await _next(context);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            if (uowAttr.RollbackOnException)
            {
                await transaction.RollbackAsync();
            }
            throw;
        }
    }
}