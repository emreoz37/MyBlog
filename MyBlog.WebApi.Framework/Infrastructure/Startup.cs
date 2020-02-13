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
using System;
using System.IO;
using System.Linq;

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
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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

            services.ConfigureCors();
            // services.ConfigureIISIntegration();

            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

            //add Easy caching
            services.AddEasyCaching(option =>
            {
                //use memory cache
                option.UseInMemory("MyBlog_memory_cache");
            });

            //add distributed memory cache
            services.AddDistributedMemoryCache();

            services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "My Blog Api",
                    Contact = new OpenApiContact
                    {
                        Name = "Emre Oz",
                        Email = "emreoz3734@gmail.com",
                        Url = new Uri("https://github.com/emreoz37")
                    }
                });
            });


            Log.Logger = new LoggerConfiguration()
                   .ReadFrom.Configuration(config)
                   .CreateLogger();

            //Disable automatic 400 respone
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

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

            services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));

        }

        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        /// <param name="loggerFactory">Logger Factory</param>
        public virtual void Configure(IApplicationBuilder application, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.UseResponseCompression();

            //easy caching
            application.UseEasyCaching();

            //Swagger   
            application.UseSwagger();

            application.UseStaticFiles();

            application.UseCors("CorsPolicy");

            application.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            application.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Blog Api v1");
            });

            loggerFactory.AddSerilog();

            application.ConfigureCustomExceptionMiddleware();

            //app.UseHttpsRedirection();
            application.UseMvc();

        }
    }
}