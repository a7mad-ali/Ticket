using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
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
            services.PostConfigure<EmailOptions>(options =>
            {
                if (string.IsNullOrWhiteSpace(options.AdminEmail))
                {
                    options.AdminEmail = options.SenderEmail;
                }

                if (string.IsNullOrWhiteSpace(options.SenderPassword))
                {
                    options.SenderPassword = options.SenderPassword;
                }

                if (string.IsNullOrWhiteSpace(options.SenderEmail))
                {
                    options.SenderEmail = options.SenderEmail;
                }
            });

            // Repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IEmailSender, MailKitEmailSender>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IUserService, UserService>();

            // AutoMapper
            // Use only the assembly containing the mapping profiles to avoid ReflectionTypeLoadException
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
