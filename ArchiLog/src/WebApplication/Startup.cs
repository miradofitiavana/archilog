using System;
using System.IO;
using System.Reflection;
using APILibrary.core.Helpers;
using APILibrary.core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.Data;
using WebApplication.Services;

namespace WebApplication
{
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
            services.AddControllers();

            // ajout de la dép. EatDbContext. Configuration avec le type de bdd et chaine de connexion
            services.AddDbContext<EatDbContext>(db =>
            db.UseLoggerFactory(EatDbContext.SqlLogger)
                .UseSqlServer(Configuration.GetConnectionString("EatConnectionString"))
            );

            /* AUTH */
            services.AddCors();

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IAuthenticationService, UserService>();
            /* FIN AUTH */

            /* DOC */
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "ArchiLog API",
                    Description = "API for showing Swagger",
                    Version = "v1"
                });
                // Set the comments path for the Swagger JSON and UI.
                var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
                options.IncludeXmlComments(filePath);
            });
            /* FIN DOC */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            /* AUTH */
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddlewareAPI>();
            /* FIN AUTH */

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /* DOC */
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ArchiLog API");
                options.RoutePrefix = "";

            });
            /* FIN DOC */
        }
    }
}
