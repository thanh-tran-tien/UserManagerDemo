using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagerDemo.Application.Common.Interface;
using UserManagerDemo.Infrastructure.Persistence;
using UserManagerDemo.Infrastructure.Repositories;

namespace UserManagerDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Lấy connection string từ appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        return services;
    }
}