using ManageUser.Application;
using ManageUser.Infrastructure;
using ManageUser.MIddlewares;

namespace ManageUser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddPresentationServices(builder.Configuration);
            //builder.Services.AddCqrs(new[] { typeof(Program).Assembly });

            // Configuración de logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(); // Agrega el proveedor de consola
            builder.Logging.AddDebug();   // Agrega el proveedor de depuración

            // Add services to the container.
            builder.Services.AddControllers();
                
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseCors("AllowAll"); // Si configuraste CORS
            
            // ManageUser/Program.cs
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication(); // Debe ir antes de Authorization
            app.UseAuthorization();



            app.MapControllers();

            app.Run();
        }
    }
}
