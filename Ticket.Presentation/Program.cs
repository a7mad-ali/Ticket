using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ticket.Infrastructure.Data;
using Ticket.Infrastructure.DependencyInjection;
using Ticket.Infrastructure.Services;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Presentation.RealTime;

namespace Ticket.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddApplicationDependencies(builder.Configuration);
            builder.Services.AddScoped<ITicketNotifier, TicketNotifier>();

            var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? new JwtOptions();
            if (!string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
            {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtOptions.Issuer),
                            ValidateAudience = !string.IsNullOrWhiteSpace(jwtOptions.Audience),
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtOptions.Issuer,
                            ValidAudience = jwtOptions.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                            ClockSkew = TimeSpan.FromMinutes(1)
                        };
                    });
            }

            // Use built-in OpenAPI support for .NET 10
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<TicketHub>("/hubs/tickets");

            app.Run();
        }
    }
}
