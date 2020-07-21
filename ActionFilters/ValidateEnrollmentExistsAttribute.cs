using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMgmtAPI.ActionFilters
{
    public class ValidateEnrollmentExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateEnrollmentExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT") || method.Equals("PATCH")) ? true : false;

            var sectionId = (Guid)context.ActionArguments["sectionId"];
            var section = await _repository.Section.GetSectionsAsync(sectionId, false);

            if (section == null)
            {
                _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (Guid)context.ActionArguments["id"];
            var enrollment = await _repository.Enrollment.GetEnrollmentAsync(sectionId, id, trackChanges);

            if (enrollment == null)
            {
                _logger.LogInfo($"Enrollment with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("enrollment", enrollment); 
                await next();
            }
        }
    }
}


