using System.Collections.Generic;
using System.Linq;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Security;
using EdFi.OneRoster.WebApi.Services;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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


            var clients = Configuration.GetSection("ApplicationSettings:Clients").Get<Dictionary<string, string>>();

            var oauth1Clients= clients.Select(m => new OAuth1Client(){Client_Id = m.Key,Client_Secret = m.Value ,Name = "random"}).ToList();

            services.AddAuthentication().AddOAuth1(
                m => { m.oAuth1Clients = oauth1Clients; }
            );

            //services.AddAuthentication(
            //    m =>
            //    {
            //        m.DefaultAuthenticateScheme = "OAuth1";
            //        m.DefaultChallengeScheme= "OAuth1";
            //        m.DefaultScheme = "OAuth1";
            //        m.AddScheme<Oauth1AuthenticationHandler>("OAuth1", null);

            //    }

            
            //    //x=> x.DefaultAuthenticateScheme = "OAuth1";
            //    //x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            
            //    );

            //    .AddOAuth1("OAuth1", options =>
            //{
            //    options.oAuth1Clients = oauth1Clients;
            //})

            services.AddSingleton<IAuthenticationService, ChallengeOnlyAuthenticationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase(new PathString("/campus/oneroster/go/ims/oneroster/v1p1/"));

            app.UseDeveloperExceptionPage();

            //loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            var loggerPath = Configuration.GetValue<string>("ApplicationSettings:LoggerPath");

            loggerFactory.AddFile(loggerPath);

            //app.UseMiddleware<RequestLoggingMiddleware>();
            //app.UseMiddleware<OAuth1Middleware>();
            //app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
