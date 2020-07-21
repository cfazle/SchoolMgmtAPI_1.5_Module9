using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMgmtAPI.ActionFilters
{
    public class ValidateAssignmentExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateAssignmentExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT") || method.Equals("PATCH")) ? true : false;

            var enrollmentId = (Guid)context.ActionArguments["enrollmentId"];
            var enrollment = await _repository.Enrollment.GetEnrollmentsAsync(enrollmentId, false);

            if (enrollment == null)
            {
                _logger.LogInfo($"Enrollment with id: {enrollmentId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (Guid)context.ActionArguments["id"];
            var assignment = await _repository.Assignment.GetAssignmentAsync(enrollmentId, id, trackChanges);

            if (assignment == null)
            {
                _logger.LogInfo($"Assignment with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("assignment", assignment);
                await next();
            }
        }
    }
}

