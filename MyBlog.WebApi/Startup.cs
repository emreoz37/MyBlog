using Core;
using Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MyBlog.WebApi
{
    public class Startup
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Ctor

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //create default file provider
            CommonHelper.DefaultFileProvider = new ProjectFileProvider(_hostingEnvironment);


            var engine = StartupEngineContext.Create();
            var serviceProvider = engine.ConfigureServices(services, _configuration);

            return serviceProvider;
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            StartupEngineContext.Current.ConfigureRequestPipeline(application, _hostingEnvironment);
        }
    }
}
