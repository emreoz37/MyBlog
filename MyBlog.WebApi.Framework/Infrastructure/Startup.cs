﻿using Data;
using EasyCaching.Core;
using EasyCaching.InMemory;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
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

            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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
            application.UseResponseCompression();

            //easy caching
            application.UseEasyCaching();

            //Swagger   
            application.UseSwagger();

            application.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Blog Api v1");
            });

            loggerFactory.AddSerilog();


            if (env.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            //app.UseHttpsRedirection();
            application.UseMvc();

        }
    }
}