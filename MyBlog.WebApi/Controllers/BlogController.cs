using Core;
using Core.Domain.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MyBlog.WebApi.DTOs;
using MyBlog.WebApi.DTOs.Blogs;
using MyBlog.WebApi.Factories;
using MyBlog.WebApi.Infrastructure.Mapper.Extensions;
using Newtonsoft.Json;
using Services;
using Services.Blogs;
using Services.Helpers;
using System;

namespace MyBlog.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        #region Fields
        private readonly IBlogService _blogService;
        private readonly IBlogFactory _blogFactory;
        private readonly ILogger<BlogController> _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        #endregion

        #region Ctor
        public BlogController(IBlogService blogService,
            IBlogFactory blogFactory,
             ILogger<BlogController> logger,
             IDateTimeHelper dateTimeHelper)
        {
            _blogService = blogService;
            _blogFactory = blogFactory;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
        }
        #endregion

        #region Methods

        #region BlogPost
        [HttpGet("{id}", Name = "PostById")]
        public IActionResult GetPostId(int id)
        {
            try
            {
                var blogPost = _blogService.GetBlogPostById(id);
                if (blogPost == null)
                {
                    _logger.LogError("BlogPost object sent from client is null.");
                    return NotFound();
                }

                _logger.LogInformation($"Returned blog with id: {id}");
                var blogPostDto = new BlogPostDto();
                _blogFactory.PrepareBlogPostResponse(blogPostDto, blogPost, false);
                return Ok(blogPostDto);

            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController GetPostId Method. Something went wrong. Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{id}/comments/")]
        public IActionResult GetPostWithComments(int id)
        {
            try
            {
                var blogPost = _blogService.GetBlogPostById(id);
                if (blogPost == null)
                {
                    _logger.LogError("BlogPost object sent from client is null.");
                    return NotFound();
                }

                _logger.LogInformation($"Returned blog with comments id: {id}");
                var blogPostDto = new BlogPostDto();
                _blogFactory.PrepareBlogPostResponse(blogPostDto, blogPost, true);
                return Ok(blogPostDto);

            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController GetPostId Method. Something went wrong. Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Post/Create")]
        public IActionResult CreatePost([FromBody]BlogPostForCreationDto blogPost)
        {
            try
            {
                if (blogPost == null)
                {
                    _logger.LogError("BlogPost object sent from client is null.");
                    return BadRequest("BlogPost object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid blogpost object sent from client.");
                    return BadRequest("Invalid model object");
                }


                var blogPostEntity = blogPost.ToEntity<BlogPost>();
                blogPostEntity.CreatedOnUtc = DateTime.UtcNow;
                _blogService.InsertBlogPost(blogPostEntity);

                var blogPostDto = new BlogPostDto();
                _blogFactory.PrepareBlogPostResponse(blogPostDto, blogPostEntity, false);

                return CreatedAtRoute("PostById", new { id = blogPostDto.Id }, blogPostDto);

            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController PostCreate Method. Something went wrong. Request : { JsonConvert.SerializeObject(blogPost) } Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut("Post/Edit/{id}")]
        public IActionResult PostEdit(int id, [FromBody] BlogPostForUpdateDto blogPost)
        {
            try
            {
                if (blogPost == null)
                {
                    _logger.LogError("BlogPost object sent from client is null.");
                    return BadRequest("BlogPost object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid blogpost object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var blogPostEntity = _blogService.GetBlogPostById(id);
                if (blogPostEntity == null)
                {
                    _logger.LogError($"BlogController PostEdit Method : Blog Post with  id {id}, hasn't been found in db.");
                    return NotFound();
                }

                blogPostEntity = blogPost.ToEntity(blogPostEntity);
                _blogService.UpdateBlogPost(blogPostEntity);

                return NoContent();
            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController PostEdit Method. Something went wrong. Request : { JsonConvert.SerializeObject(blogPost) } Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpDelete("Post/Delete/{id}")]
        public IActionResult PostDelete(int id)
        {
            try
            {
                //try to get a blog post with the specified id
                var blogPost = _blogService.GetBlogPostById(id);
                if (blogPost == null)
                {
                    _logger.LogError($"BlogController PostEdit Method : Blog Post with  id {id}, hasn't been found in db.");
                    return NotFound();
                }

                _blogService.DeleteBlogPost(blogPost);
                return NoContent();
            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController PostDelete Method. Something went wrong. Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region BlogComment

        [HttpGet("Comment/{id}", Name = "CommentById")]
        public IActionResult GetCommentById(int id)
        {
            try
            {
                var blogComment = _blogService.GetBlogCommentById(id);
                if (blogComment == null)
                {
                    _logger.LogError("BlogComment object sent from client is null.");
                    return NotFound();
                }

                _logger.LogInformation($"Returned blog comment with id: {id}");

                var blogCommentDto = _blogFactory.PrepareBlogPostCommentModel(blogComment);
                return Ok(blogCommentDto);

            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController GetCommentById Method. Something went wrong. Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpPost("Comment/Add")]
        public IActionResult AddComment([FromBody]BlogCommentForCreationDto blogComment)
        {

            try
            {
                if (blogComment == null)
                {
                    _logger.LogError("BlogComment object sent from client is null.");
                    return BadRequest("BlogComment object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid blogComment object sent from client.");
                    return BadRequest("Invalid model object");
                }


                var blogCommentEntity = blogComment.ToEntity<BlogComment>();
                blogCommentEntity.CreatedOnUtc = DateTime.UtcNow;
                _blogService.InsertBlogComment(blogCommentEntity);

                var blogCommentDto =  _blogFactory.PrepareBlogPostCommentModel(blogCommentEntity);

                return CreatedAtRoute("CommentById", new { id = blogCommentDto.Id }, blogCommentDto);

            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController PostCreate Method. Something went wrong. Request : { JsonConvert.SerializeObject(blogComment) } Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }


        }

        [HttpPut("Comment/Edit/{id}")]
        public IActionResult CommentEdit(int id, [FromBody]BlogCommentForUpdateDto blogComment)
        {
            try
            {
                if (blogComment == null)
                {
                    _logger.LogError("BlogComment object sent from client is null.");
                    return BadRequest("BlogComment object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BlogComment object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var blogCommentEntity = _blogService.GetBlogCommentById(id);
                if (blogCommentEntity == null)
                {
                    _logger.LogError($"BlogController CommentEdit Method : Blog Comment with  id {id}, hasn't been found in db.");
                    return NotFound();
                }

                blogCommentEntity = blogComment.ToEntity(blogCommentEntity);
                _blogService.UpdateBlogComment(blogCommentEntity);

                return NoContent();
            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController CommentEdit Method. Something went wrong. Request : { JsonConvert.SerializeObject(blogComment) } Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }
           
        }

        [HttpDelete("Comment/Delete/{id}")]
        public IActionResult CommentDelete(int id)
        {
            try
            {
                //try to get a blog comment with the specified id
                var blogComment = _blogService.GetBlogCommentById(id);
                if (blogComment == null)
                {
                    _logger.LogError($"BlogController CommentDelete Method : Blog Comment with  id {id}, hasn't been found in db.");
                    return NotFound();
                }

                _blogService.DeleteBlogComment(blogComment);
                return NoContent();
            }
            catch (Exception ex)
            {
                var logMessage = $"BlogController CommentDelete Method. Something went wrong. Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #endregion
    }
}
