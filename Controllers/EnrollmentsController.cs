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
    [Route("api/v1/organizations/{orgId}/users/{userId}/courses/{courseId}/sections/{sectionId}/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EnrollmentsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task < IActionResult> GetEnrollmentsForSection(Guid sectionId)

        {
            var section = await _repository.Section.GetSectionsAsync(sectionId, trackChanges: false);

            if (section == null)
            {
                _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                return NotFound();
            }

            var enrollmentsFromDb = await _repository.Enrollment.GetEnrollmentsAsync(sectionId, trackChanges: false);

            var enrollmentsDto = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollmentsFromDb);

            return Ok(enrollmentsDto);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetEnrollmentForSection(Guid sectionId, Guid id)
        {
            var organization = await _repository.Section.GetSectionsAsync(sectionId, trackChanges: false);
            if (organization == null)
            {
                _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                return NotFound();
            }

            var enrollmentDb =await _repository.Enrollment.GetEnrollmentAsync(sectionId, id, trackChanges: false);
            if (enrollmentDb == null)
            {
                _logger.LogInfo($"Enrollment with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var enrollment = _mapper.Map<EnrollmentDto>(enrollmentDb);

            return Ok(enrollment);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]

        public async Task < IActionResult> CreateEnrollmentForSection(Guid sectionId, [FromBody] EnrollmentForCreationDto enrollment)
        {
           /* if (enrollment == null)
            {
                _logger.LogError("EnrollmentForCreationDto object sent from client is null.");
                return BadRequest("EnrollmentForCreationDto object is null");
            }

            //    var organization = _repository.Company.GetCompany(companyId, trackChanges: false);
            var section = await _repository.Section.GetSectionsAsync(sectionId, trackChanges: false);

            if (section == null)
            {
                _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                return NotFound();
            }
           */

            var enrollmentEntity = _mapper.Map<Enrollment>(enrollment);

            //      _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            _repository.Enrollment.CreateEnrollmentForSection(sectionId, enrollmentEntity);
           await _repository.SaveAsync();

            var enrollmentToReturn = _mapper.Map<EnrollmentDto>(enrollmentEntity);

            return CreatedAtRoute(new { sectionId, id = enrollmentToReturn.Id }, enrollmentToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEnrollmentExistsAttribute))]
        public async Task <IActionResult> DeleteEnrollmentForSection(Guid sectionId, Guid id)
        {
            /*  var section = await _repository.Section.GetSectionsAsync(sectionId, trackChanges: false);
              if (section == null)
              {
                  _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                  return NotFound();
              }

              var enrollmentForSection =await  _repository.Enrollment.GetEnrollmentAsync(sectionId, id, trackChanges: false);
              if (enrollmentForSection == null)
              {
                  _logger.LogInfo($"Enrollment with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */
            var enrollmentForSection = HttpContext.Items["enrollment"] as Enrollment;
            _repository.Enrollment.DeleteEnrollment(enrollmentForSection);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEnrollmentExistsAttribute))]
        public async Task <IActionResult> UpdateEnrollmentForSection(Guid sectionId, Guid id, [FromBody] EnrollmentForUpdateDto enrollment)
        {
            /*  if (enrollment == null)
              {
                  _logger.LogError("EnrollmentForUpdateDto object sent from client is null.");
                  return BadRequest("EnrollmentForUpdateDto object is null");
              }

              if (!ModelState.IsValid)
              {
                  _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                  return UnprocessableEntity(ModelState);
              }


              var section =await _repository.Section.GetSectionsAsync(sectionId, trackChanges: false);
              if (section == null)
              {
                  _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                  return NotFound();
              } 

              var enrollmentEntity =await _repository.Enrollment.GetEnrollmentAsync(sectionId, id, trackChanges: true);
              if (enrollmentEntity == null)
              {
                  _logger.LogInfo($"Section with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

            var enrollmentEntity = HttpContext.Items["enrollment"] as Enrollment;
            

            _mapper.Map(enrollment, enrollmentEntity);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateEnrollmentExistsAttribute))]
        public async Task < IActionResult> PartiallyUpdateEnrollmentForSection(Guid sectionId, Guid id, [FromBody] JsonPatchDocument<EnrollmentForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            /*   var section = await _repository.Section.GetSectionsAsync(sectionId, trackChanges: false);
               if (section == null)
               {
                   _logger.LogInfo($"Section with id: {sectionId} doesn't exist in the database.");
                   return NotFound();
               }

               var enrollmentEntity = await _repository.Enrollment.GetEnrollmentAsync(sectionId, id, trackChanges: true);
               if (enrollmentEntity == null)
               {
                   _logger.LogInfo($"Enrollment with id: {id} doesn't exist in the database.");
                   return NotFound();
               } */
            var enrollmentEntity = HttpContext.Items["enrollment"] as Enrollment;
            var enrollmentToPatch = _mapper.Map<EnrollmentForUpdateDto>(enrollmentEntity);

            patchDoc.ApplyTo(enrollmentToPatch, ModelState);

            TryValidateModel(enrollmentToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            patchDoc.ApplyTo(enrollmentToPatch);

            _mapper.Map(enrollmentToPatch, enrollmentEntity);

           await _repository.SaveAsync();

            return NoContent();
        }
    }
}
