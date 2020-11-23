using EdFi.OneRoster.Persistence.EntityFrameWork;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Security;
using EdFi.OneRoster.WebApi.Services;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace EdFi.OneRoster.WebApi
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
            var settings = Configuration.GetSection("ApplicationSettings"); 
            services.Configure<ApplicationSettings>(settings);

            var serviceName = Configuration.GetValue<string>("ApplicationSettings:OneRosterService");

            if (serviceName == "OneRosterStaticDataService")
                services.AddScoped<IOneRosterService, OneRosterStaticDataService>();
            else
                services.AddScoped<IOneRosterService, OneRosterDatabaseService>();

            services.AddDbContext<EdfiContext>(options =>options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLConnection")));

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            }); ;

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Oauth1 client", new OpenApiSecurityScheme(){In = ParameterLocation.Header,Description = "client name",Name = "ClientName"});
                c.AddSecurityDefinition("Oauth1 secret", new OpenApiSecurityScheme() { In = ParameterLocation.Header, Description = "client secret", Name = "ClientSecret" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase(new PathString("/campus/oneroster/go/ims/oneroster/v1p1/"));

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json","MyAPI");
            });

            app.UseDeveloperExceptionPage();

            //loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            var loggerPath = Configuration.GetValue<string>("ApplicationSettings:LoggerPath");

            loggerFactory.AddFile(loggerPath);

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<OAuth1Middleware>();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

         


        }
    }
}
