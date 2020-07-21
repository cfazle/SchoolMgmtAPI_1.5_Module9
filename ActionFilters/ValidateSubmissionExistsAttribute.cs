using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMgmtAPI.ActionFilters
{
    public class ValidateSubmissionExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateSubmissionExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT") || method.Equals("PATCH")) ? true : false;

            var assignmentId = (Guid)context.ActionArguments["assignmentId"];
            var assignment = await _repository.Assignment.GetAssignmentsAsync(assignmentId, false);

            if (assignment == null)
            {
                _logger.LogInfo($"Assignment with id: {assignmentId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (Guid)context.ActionArguments["id"];
            var submission = await _repository.Submission.GetSubmissionAsync(assignmentId, id, trackChanges);

            if (submission == null)
            {
                _logger.LogInfo($"Submission with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("submission", submission);
                await next();
            }
        }
    }
}

