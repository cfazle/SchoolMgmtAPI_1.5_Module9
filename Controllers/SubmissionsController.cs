using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SchoolMgmtAPI.ActionFilters;

namespace SchoolMgmtAPI.Controllers
{
    [Route("api/v1/organizations/{orgId}/users/{userId}/courses/{courseId}/sections/{sectionId}/enrollments" +
        "/{enrollmentId}/assignments/{assignmentId}/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        
            private readonly IRepositoryManager _repository;
            private readonly ILoggerManager _logger;
            private readonly IMapper _mapper;

            public SubmissionsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
            {
                _repository = repository;
                _logger = logger;
                _mapper = mapper;
            }

            [HttpGet]
            public async Task < IActionResult> GetSubmissionsForAssignment(Guid assignmentId)

            {
                var assignment = await _repository.Assignment.GetAssignmentsAsync(assignmentId, trackChanges: false);

                if (assignment == null)
                {
                    _logger.LogInfo($"Assignment with id: {assignmentId} doesn't exist in the database.");
                    return NotFound();
                }

                var submissionsFromDb =await _repository.Submission.GetSubmissionsAsync(assignmentId, trackChanges: false);

                var submissionsDto = _mapper.Map<IEnumerable<SubmissionDto>>(submissionsFromDb);

                return Ok(submissionsDto);
            }
        [HttpGet("{id}")]
        public  async Task <IActionResult> GetSubmisionForAssignment(Guid assignmentId, Guid id)
        {
            var enrollment = await _repository.Assignment.GetAssignmentsAsync(assignmentId, trackChanges: false);
            if (enrollment == null)
            {
                _logger.LogInfo($"Assignment with id: {assignmentId} doesn't exist in the database.");
                return NotFound();
            }

            var submissionDb = await _repository.Submission.GetSubmissionAsync(assignmentId, id, trackChanges: false);
            if (submissionDb == null)
            {
                _logger.LogInfo($"Submission with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var submission = _mapper.Map<SubmissionDto>(submissionDb);

            return Ok(submission);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task <IActionResult> CreateSubmissionForAssinment(Guid assignmentId,
            [FromBody] SubmissionForCreationDto submission)
        {
            if (submission == null)
            {
                _logger.LogError("SubmissionForCreationDto object sent from client is null.");
                return BadRequest("SubmissionForCreationDto object is null");
            }

            //    var organization = _repository.Company.GetCompany(companyId, trackChanges: false);
            var assignment = await _repository.Assignment.GetAssignmentsAsync(assignmentId, trackChanges: false);

            if (assignment == null)
            {
                _logger.LogInfo($"Section with id: {assignmentId} doesn't exist in the database.");
                return NotFound();
            }


            var submissionEntity = _mapper.Map<Submission>(submission);

            //      _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            _repository.Submission.CreateSubmissionForAssignment(assignmentId, submissionEntity);
          await  _repository.SaveAsync();

            var submissionToReturn = _mapper.Map<SubmissionDto>(submissionEntity);

            return CreatedAtRoute(new {assignmentId, id = submissionToReturn.Id }, submissionToReturn);
        }
      
        
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateSubmissionExistsAttribute))]
        public async Task <IActionResult> DeleteSubissionForAssignment(Guid assignmentId, Guid id)
        {
            /*  var assignment = await _repository.Assignment.GetAssignmentsAsync(assignmentId, trackChanges: false);
              if (assignment == null)
              {
                  _logger.LogInfo($"Enrollment with id: {assignmentId} doesn't exist in the database.");
                  return NotFound();
              }

              var submissionForAssignment = await _repository.Submission.GetSubmissionAsync(assignmentId, id, trackChanges: false);
              if (submissionForAssignment == null)
              {
                  _logger.LogInfo($"Submission with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

            var submissionForAssignment = HttpContext.Items["submission"] as Submission;

            _repository.Submission.DeleteSubmission(submissionForAssignment);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateSubmissionExistsAttribute))]
        public async Task <IActionResult> UpdateSubmissionForAssignment(Guid assignmentId, Guid id, [FromBody] SubmissionForUpdateDto submission)
        {
            /*  if (submission == null)
              {
                  _logger.LogError("SubmissionForUpdateDto object sent from client is null.");
                  return BadRequest("Submission ForUpdateDto object is null");
              }

              if (!ModelState.IsValid)
              {
                  _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                  return UnprocessableEntity(ModelState);
              }


              var assignment =await _repository.Assignment.GetAssignmentsAsync(assignmentId, trackChanges: false);
              if (assignment == null)
              {
                  _logger.LogInfo($"Assignment with id: {assignmentId} doesn't exist in the database.");
                  return NotFound();
              }

              var submissionEntity =await _repository.Submission.GetSubmissionAsync(assignmentId, id, trackChanges: true);
              if (submissionEntity == null)
              {
                  _logger.LogInfo($"Submission with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

           
            var submissionEntity = HttpContext.Items["submission"] as Submission;

            _mapper.Map(submission, submissionEntity);
           await  _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateSubmissionExistsAttribute))]
        public async Task <IActionResult> PartiallyUpdatesubmissionForAssignment(Guid assignmentId, Guid id, [FromBody] JsonPatchDocument<SubmissionForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            /*  var assignment =await _repository.Assignment.GetAssignmentsAsync(assignmentId, trackChanges: false);
              if (assignment == null)
              {
                  _logger.LogInfo($"Assignment with id: {assignmentId} doesn't exist in the database.");
                  return NotFound();
              }

              var submissionEntity = await _repository.Submission.GetSubmissionAsync(assignmentId, id, trackChanges: true);
              if (submissionEntity == null)
              {
                  _logger.LogInfo($"Submission with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

            var submissionEntity = HttpContext.Items["submission"] as Submission;

            var submissionToPatch = _mapper.Map<SubmissionForUpdateDto>(submissionEntity);

            patchDoc.ApplyTo(submissionToPatch, ModelState);

            TryValidateModel(submissionToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            patchDoc.ApplyTo(submissionToPatch);

            _mapper.Map(submissionToPatch, submissionEntity);

           await _repository.SaveAsync();

            return NoContent();
        }

    }
    }
