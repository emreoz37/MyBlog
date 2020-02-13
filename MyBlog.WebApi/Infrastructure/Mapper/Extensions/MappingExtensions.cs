using Core;
using Core.Infrastructure.Mapper;
using MyBlog.WebApi.Framework.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.WebApi.Infrastructure.Mapper.Extensions
{
    /// <summary>
    /// Represents the extensions to map entity to Dto and vise versa
    /// </summary>
    public static class MappingExtensions
    {
        #region Utilities

        /// <summary>
        /// Execute a mapping from the source object to a new destination object. The source type is inferred from the source object
        /// </summary>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        private static TDestination Map<TDestination>(this object source)
        {
            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// </summary>
        /// <typeparam name="TSource">Source object type</typeparam>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <returns>Mapped destination object, same instance as the passed destination object</returns>
        private static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #endregion

        #region Methods

        #region Dto-Entity mapping

        /// <summary>
        /// Execute a mapping from the entity to a new Dto
        /// </summary>
        /// <typeparam name="TDto">Dto type</typeparam>
        /// <param name="entity">Entity to map from</param>
        /// <returns>Mapped Dto</returns>
        public static TDto ToDto<TDto>(this BaseEntity entity) where TDto : BaseEntityDto
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity.Map<TDto>();
        }

        /// <summary>
        /// Execute a mapping from the entity to the existing Dto
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TDto">Dto type</typeparam>
        /// <param name="entity">Entity to map from</param>
        /// <param name="Dto">Dto to map into</param>
        /// <returns>Mapped Dto</returns>
        public static TDto ToDto<TEntity, TDto>(this TEntity entity, TDto Dto)
            where TEntity : BaseEntity where TDto : BaseEntityDto
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (Dto == null)
                throw new ArgumentNullException(nameof(Dto));

            return entity.MapTo(Dto);
        }

        /// <summary>
        /// Execute a mapping from the Dto to a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="Dto">Dto to map from</param>
        /// <returns>Mapped entity</returns>
        public static TEntity ToEntity<TEntity>(this BaseRequestDto Dto) where TEntity : BaseEntity
        {
            if (Dto == null)
                throw new ArgumentNullException(nameof(Dto));

            return Dto.Map<TEntity>();
        }

        /// <summary>
        /// Execute a mapping from the Dto to the existing entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TDto">Dto type</typeparam>
        /// <param name="Dto">Dto to map from</param>
        /// <param name="entity">Entity to map into</param>
        /// <returns>Mapped entity</returns>
        public static TEntity ToEntity<TEntity, TDto>(this TDto Dto, TEntity entity)
            where TEntity : BaseEntity where TDto : BaseRequestDto
        {
            if (Dto == null)
                throw new ArgumentNullException(nameof(Dto));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Dto.MapTo(entity);
        }


        #endregion

        #endregion
    }
}