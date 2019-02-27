using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestWebApplication.Conventions
{
    public class ApiControllerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!(controller.ControllerType.IsDefined(typeof(ApiVersionAttribute)) || controller.ControllerType.IsDefined(typeof(ApiVersionNeutralAttribute))))
            {
                if (controller.Attributes is List<object>
                    attributes)
                {
                    attributes.Add(new ApiVersionNeutralAttribute());
                }
            }
        }
    }

    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddApiControllerVersion(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Services.Configure<MvcOptions>(options => options.Conventions.Add(new ApiControllerVersionConvention()));
            return builder;
        }
    }

    public class ApiRequestModelValidFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}