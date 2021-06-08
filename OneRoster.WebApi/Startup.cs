using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Security;
using EdFi.OneRoster.WebApi.Services;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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

            if (serviceName == "OneRosterStaticService")
                services.AddScoped<IOneRosterService, OneRosterStaticDataService>();
            else
                services.AddScoped<IOneRosterService, OneRosterDatabaseService>();


            services.AddScoped<IIdentityService, IdentityService>();

            var dbMode = Configuration.GetValue<string>("ApplicationSettings:DbMode");
            if (dbMode == "MsSQL")
            {
                services.AddDbContext<EdfiContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLConnection")));
            }
            else
            {
                services.AddDbContext<EdfiContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLConnection")));
            }

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            }); ;


            var clients = Configuration.GetSection("ApplicationSettings:Clients").Get<Dictionary<string, string>>();
            var oauth1Clients = clients.Select(m => new OAuth1Client() { Client_Id = m.Key, Client_Secret = m.Value, Name = "random" }).ToList();

            services.AddAuthentication(OAuth1Options.DefaultScheme).AddOAuth1(m => { m.oAuth1Clients = oauth1Clients; });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,

                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Jwt:Issuer"],
                       ValidAudience = Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                   };

                   //Configure();
               });
            services.AddSwaggerGen();

            //ConfigureSwaggerService(services);
        }

        private void ConfigureSwaggerService(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectAMP API", Version = "v1" });
                //var filePath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //filePath = Path.Combine(AppContext.BaseDirectory, filePath);
                //c.IncludeXmlComments(filePath);
                //c.DocumentFilter<CustomSwashbuckleDocumentFilter>();

                c.AddSecurityDefinition("oauth2_client_credentials", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Project Amp OAuth 2.0 Client Credentials Grant Type authorization",
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/OAuth/token", UriKind.Relative)
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2_client_credentials"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }


        private void SwaggerConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Clever API V1");
                //c.OAuthClientId("aj7a7suusj");
                //c.OAuthClientSecret("ks98s8a8sas");
                c.OAuthAppName("Swagger Api Calls");
                c.RoutePrefix = string.Empty;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //SwaggerConfigure(app, env);

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase(new PathString("/campus/oneroster/go/ims/oneroster/v1p1/"));

            app.UseDeveloperExceptionPage();
                        
            var loggerPath = Configuration.GetValue<string>("ApplicationSettings:LoggerPath");

            loggerFactory.AddFile(loggerPath);

            app.UseMiddleware<RequestLoggingMiddleware>();
            
            app.UseHttpsRedirection();


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
