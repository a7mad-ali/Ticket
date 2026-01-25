using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Interfaces.IRepository;
using Ticket.Infrastructure.Data;
using Ticket.Infrastructure.Repositories;
using Ticket.Infrastructure.Services;

namespace Ticket.Presentation.DependencyInjection
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplicationDependencies(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<TicketDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Default")));

            // Repositories
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<TicketService>();
            services.AddScoped<UserService>();

            // AutoMapper
            // Use only the assembly containing the mapping profiles to avoid ReflectionTypeLoadException
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
