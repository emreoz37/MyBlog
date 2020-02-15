using Core.Infrastructure;
using Data;
using EasyCaching.Core;
using EasyCaching.InMemory;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyBlog.WebApi.Framework.Filters;
using MyBlog.WebApi.Framework.GlobalErrorHandling.Extensions;
using MyBlog.WebApi.Framework.Infrastructure.Extensions;
using Serilog;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyBlog.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring common features and middleware on application startup
    /// </summary>
    public class Startup : Core.Infrastructure.IStartup
    {
        public int Order => 1;

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type Finder</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, ITypeFinder typeFinder, IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            services.AddDbContextPool<ApplicationDbContext>(optionsBuilder =>
            {
                var dbContextOptionsBuilder = optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            });

            //add EF services
            services.AddEntityFrameworkSqlServer();
            services.AddEntityFrameworkProxies();

            //Disable automatic 400 respone
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //compression
            services.AddResponseCompression();

            //add Easy caching
            services.AddEasyCaching(option =>
            {
                //use memory cache
                option.UseInMemory("MyBlog_memory_cache");
            });

            //add distributed memory cache
            services.AddDistributedMemoryCache();

            services.ConfigureCors();
            // services.ConfigureIISIntegration();

            //add options feature
            services.AddOptions();

            var appSettingsSection = config.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // JWT authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));

            services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "My Blog Api",
                    Description = ".Net Developer Evaluation Project For Digiturk Company",
                    Contact = new OpenApiContact
                    {
                        Name = "Emre Oz",
                        Email = "emreoz3734@gmail.com",
                        Url = new Uri("https://github.com/emreoz37")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. <br>
                      Enter 'Bearer' [space] and then your token in the text input below. <br>
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                 },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                                },
                        new List<string>()
                    }
                });

                //TODO: Could dynamically find here.
                var xmlFile = typeFinder.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "MyBlog.WebApi");
                if (xmlFile != null)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile.GetName().Name + ".XML");
                    options.IncludeXmlComments(xmlPath);
                }

            });

            Log.Logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(config)
                  .CreateLogger();

            var mvcBuilder = services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
                 .AddJsonOptions(options =>
                 {
                     options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                 })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            mvcBuilder.AddFluentValidation(cfg =>
            {
                //register all available validators from project assemblies
                var assemblies = mvcBuilder.PartManager.ApplicationParts
                    .OfType<AssemblyPart>()
                    .Select(part => part.Assembly);

                cfg.RegisterValidatorsFromAssemblies(assemblies);

            });

        }

        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        /// <param name="loggerFactory">Logger Factory</param>
        public virtual void Configure(IApplicationBuilder application)
        {
            var env = StartupEngineContext.Current.Resolve<IHostingEnvironment>();
            var loggerFactory = StartupEngineContext.Current.Resolve<ILoggerFactory>();

            if (env.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.ConfigureCustomExceptionMiddleware();

            application.UseResponseCompression();

            application.UseStaticFiles();

            //easy caching
            application.UseEasyCaching();

            application.UseCors("CorsPolicy");

            application.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            application.UseAuthentication();

            //Swagger   
            application.UseSwagger();

            application.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Blog Api v1");
            });

            loggerFactory.AddSerilog();

            //app.UseHttpsRedirection();
            application.UseMvc();

        }
    }
}