using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Zonorai.Tenants.Infrastructure.Configuration;

namespace Zonorai.Tenants.Net6WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediatR(typeof(Program).Assembly);
        services.AddZonoraiMultiTenancy(Configuration);
        services.AddMvc();
        services.AddRouting();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
            {  
                Name = "Authorization",  
                Type = SecuritySchemeType.ApiKey,  
                Scheme = "Bearer",  
                BearerFormat = "JWT",  
                In = ParameterLocation.Header,  
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",  
            });  
            c.AddSecurityRequirement(new OpenApiSecurityRequirement  
            {  
                {  
                    new OpenApiSecurityScheme  
                    {  
                        Reference = new OpenApiReference  
                        {  
                            Type = ReferenceType.SecurityScheme,  
                            Id = "Bearer"  
                        }  
                    },  
                    new string[] {}  
  
                }  
            });  
        });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMultiTenant();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            endpoints.MapSwagger();
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });


        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "My API V1");
        });
    }
}