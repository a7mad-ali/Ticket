using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Entities;
using Ticket.Infrastructure.Data;
using Ticket.Infrastructure.Repositories;
using Ticket.Infrastructure.Services;

namespace Ticket.Infrastructure.DependencyInjection
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
           

            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.PostConfigure<EmailOptions>(options =>
            {
                if (string.IsNullOrWhiteSpace(options.AdminEmail))
                {
                    options.AdminEmail = options.SenderEmail;
                }
            });

            // Repositories
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            // Services
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IEmailSender, MailKitEmailSender>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IUserService, UserService>();

            // AutoMapper
            // Use only the assembly containing the mapping profiles to avoid ReflectionTypeLoadException
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
