using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyBlog.WebApi.Framework.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.WebApi.Framework.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
          
        }

        //If you want custom 400 response
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //List<BaseErrorDto> errors = new List<BaseErrorDto>();

                //var errorsInModelState = context.ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                //var errorResponse = new BaseResult
                //{
                //    StatusCode = StatusCodes.Status400BadRequest,
                //    Message = "Validation failed. Please check the fields below."
                //};

                //foreach (var error in errorsInModelState)
                //{
                //    foreach (var subError in error.Value)
                //    {
                //        var errorDto = new BaseErrorDto
                //        {
                //            FieldName = error.Key,
                //            Message = subError

                //        };

                //        errors.Add(errorDto);
                //    }
                //}

                //errorResponse.Data = errors;

                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

    }
}
