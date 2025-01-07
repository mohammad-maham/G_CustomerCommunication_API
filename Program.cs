
using G_CustomerCommunication_API.BusinessLogics;
using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Middleware;
using G_CustomerCommunication_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace G_CustomerCommunication_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddDbContext<GCustomerCommunicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("GCustomerCommunicationDbContext"), options => options.UseNodaTime()));

            builder.Services.AddScoped<ICommunication, Communication>();
            builder.Services.AddScoped<INotificationManager, NotificationManager>();
            builder.Services.AddScoped<IAccounting, Accounting>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(option => { option.SwaggerDoc("v1", new OpenApiInfo { Title = "Gold Marketing Customer Communication API's", Version = "v1", Description = ".NET Core 8 Web API" }); });


            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
