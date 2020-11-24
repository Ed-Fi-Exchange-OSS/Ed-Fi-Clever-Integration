using EdFi.OneRoster.Persistence.EntityFrameWork;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Security;
using EdFi.OneRoster.WebApi.Services;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            if (serviceName == "OneRosterStaticService")
                services.AddScoped<IOneRosterService, OneRosterStaticDataService>();
            else
                services.AddScoped<IOneRosterService, OneRosterDatabaseService>();

            services.AddDbContext<EdfiContext>(options =>options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLConnection")));

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            }); ;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
