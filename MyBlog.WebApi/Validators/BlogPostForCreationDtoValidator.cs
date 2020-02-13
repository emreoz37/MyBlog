using FluentValidation;
using MyBlog.WebApi.DTOs.Blogs;

namespace MyBlog.WebApi.Validators
{
    public class BlogPostForCreationDtoValidator : AbstractValidator<BlogPostForCreationDto>
    {
        public BlogPostForCreationDtoValidator()
        {
            RuleFor(x => x.Title)
               .NotEmpty()
               .WithMessage("Title is required."); //TODO: Can be made based on localization

            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessage("Body is required."); //TODO: Can be made based on localization

            RuleFor(x => x.Tags)
                .Must(x => x == null || !x.Contains("."))
                .WithMessage("Dots are not supported by tags.");

        }
    }
}
