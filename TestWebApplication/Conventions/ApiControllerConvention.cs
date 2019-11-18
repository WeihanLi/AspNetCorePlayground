using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

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

    public class AllowAnonymousPolicyTransformer : IApplicationModelConvention
    {
        private readonly string _policyName;

        public AllowAnonymousPolicyTransformer() : this("anonymous")
        {
        }

        public AllowAnonymousPolicyTransformer(string policyName) => _policyName = policyName;

        public void Apply(ApplicationModel application)
        {
            foreach (var controllerModel in application.Controllers)
            {
                if (controllerModel.Filters.Any(_ => _.GetType() == typeof(AuthorizeFilter)))
                {
                    foreach (var actionModel in controllerModel.Actions)
                    {
                        if (actionModel.Filters.Any(_ => _.GetType() == typeof(AllowAnonymousFilter)))
                        {
                            var allowAnonymousFilter = actionModel.Filters.First(_ => _.GetType() == typeof(AllowAnonymousFilter));
                            actionModel.Filters.Remove(allowAnonymousFilter);
                            actionModel.Filters.Add(new AuthorizeFilter(_policyName));
                        }
                    }
                }
                else
                {
                    if (controllerModel.Filters.Any(_ => _.GetType() == typeof(AllowAnonymousFilter)))
                    {
                        var allowAnonymousFilter = controllerModel.Filters.First(_ => _.GetType() == typeof(AllowAnonymousFilter));
                        controllerModel.Filters.Remove(allowAnonymousFilter);
                    }
                    controllerModel.Filters.Add(new AuthorizeFilter(_policyName));
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

        public static IMvcBuilder AddAnonymousPolicyTransformer(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Insert(0, new AllowAnonymousPolicyTransformer());
            });
            return builder;
        }

        public static IMvcBuilder AddAnonymousPolicyTransformer(this IMvcBuilder builder, string policyName)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Insert(0, new AllowAnonymousPolicyTransformer(policyName));
            });
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
