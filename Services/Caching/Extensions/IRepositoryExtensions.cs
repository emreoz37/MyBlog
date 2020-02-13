using Core;
using Core.Caching;
using Core.Infrastructure;
using Data;

namespace Services.Caching.Extensions
{
    public static class IRepositoryExtensions
    {
        public static TEntity ToCachedGetById<TEntity>(this IRepository<TEntity> repository, object id, string cacheKey = null) where TEntity : BaseEntity
        {
            var cacheManager = StartupEngineContext.Current.Resolve<IStaticCacheManager>();

            return cacheManager.Get(cacheKey ?? string.Format(CachingDefaults.EntityCacheKey, typeof(TEntity).Name, id), () => repository.GetById(id));
        }
    }
}
