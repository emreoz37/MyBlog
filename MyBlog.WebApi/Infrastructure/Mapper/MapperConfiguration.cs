using Core.Infrastructure.Mapper;
using AutoMapper;
using Core.Domain.Blogs;
using MyBlog.WebApi.DTOs.Blogs;

namespace MyBlog.WebApi.Infrastructure.Mapper
{
    public class MapperConfiguration : Profile, IMapperProfile
    {
        public MapperConfiguration()
        {
            CreateBlogsMaps();
        }

        /// <summary>
        /// Create blogs maps 
        /// </summary>new
        protected virtual void CreateBlogsMaps()
        {
            CreateMap<BlogPostForCreationDto, BlogPost>();

            CreateMap<BlogPostForUpdateDto, BlogPost>();

            CreateMap<BlogPostDto, BlogPost>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<BlogPost, BlogPostDto>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.Tags, options => options.Ignore())
                .ForMember(model => model.NumberOfComments, options => options.Ignore())
                .ForMember(model => model.Comments, options => options.Ignore());


            #region Comments
            CreateMap<BlogCommentForCreationDto, BlogComment>();

            CreateMap<BlogCommentForUpdateDto, BlogComment>();

            CreateMap<BlogCommentDto, BlogComment>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<BlogComment, BlogCommentDto>()
                .ForMember(model => model.CustomerName, options => options.Ignore())
                .ForMember(model => model.CustomerAvatarUrl, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.AllowViewingProfiles, options => options.Ignore());

            #endregion

        }


        public int Order => 1;
    }
}
