using FluentValidation;
using MyBlog.WebApi.DTOs.Blogs;

namespace MyBlog.WebApi.Validators
{
    public class BlogCommentCreationDtoValidator : AbstractValidator<BlogCommentForCreationDto>
    {
        public BlogCommentCreationDtoValidator()
        {
            RuleFor(x => x.CommentText).NotEmpty().WithMessage("Enter comment"); //TODO: Can be made based on localization
        }
    }
}
