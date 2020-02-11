using Autofac;
using Core.Caching;
using Core.Infrastructure;
using Core.Infrastructure.DependencyManagement;
using Data;
using Microsoft.EntityFrameworkCore;
using Services.Blogs;

namespace MyBlog.WebApi.Framework.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        public virtual void Register(ContainerBuilder builder)
        {
            //file provider
            builder.RegisterType<ProjectFileProvider>().As<IProjectFileProvider>().InstancePerLifetimeScope();

            //data layer
            builder.Register(context => new ApplicationDbContext(context.Resolve<DbContextOptions<ApplicationDbContext>>()))
    .As<IDbContext>().InstancePerLifetimeScope();

            //repositories
            builder.RegisterGeneric(typeof(EntityRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //static cache manager
            builder.RegisterType<MemoryCacheManager>()
                  .As<IStaticCacheManager>()
                  .SingleInstance();

            builder.RegisterType<BlogService>().As<IBlogService>().InstancePerLifetimeScope();
        }
    }
}
