using Autofac;
using Core.Infrastructure.DependencyManagement;
using MyBlog.WebApi.Factories;

namespace MyBlog.WebApi.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 2;

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        public virtual void Register(ContainerBuilder builder)
        {
            builder.RegisterType<BlogFactory>().As<IBlogFactory>().InstancePerLifetimeScope();
        }
    }
}
