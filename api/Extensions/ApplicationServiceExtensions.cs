using api.Data;
using api.Interfaces;
using api.Services;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static  IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration cfg) {
                services.AddDbContext<ApplicationDbContext>(Options =>
                Options.UseSqlite(cfg.GetConnectionString("Default")));
                services.AddCors();
                services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}