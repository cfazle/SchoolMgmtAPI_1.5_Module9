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
    [Route("api/v1/organizations/{orgId}/users/{userId}/courses/{courseId}/sections")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public SectionsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task <IActionResult> GetSectionsForCourses(Guid courseId)

        {
            var course = await _repository.Course.GetCoursesAsync(courseId, trackChanges: false);

            if (course == null)
            {
                _logger.LogInfo($"Course with id: {courseId} doesn't exist in the database.");
                return NotFound();
            }

            var sectionsFromDb =  await _repository.Section.GetSectionsAsync(courseId, trackChanges: false);

            var sectionsDto = _mapper.Map<IEnumerable<SectionDto>>(sectionsFromDb);

            return Ok(sectionsDto);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetSectionForCourse(Guid courseId, Guid id)
        {
            var course = await _repository.Course.GetCoursesAsync(courseId, trackChanges: false);
            if (course == null)
            {
                _logger.LogInfo($"Course with id: {courseId} doesn't exist in the database.");
                return NotFound();
            }

            var sectionDb =await _repository.Section.GetSectionAsync(courseId, id, trackChanges: false);
            if (sectionDb == null)
            {
                _logger.LogInfo($"Course with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var section = _mapper.Map<SectionDto>(sectionDb);

            return Ok(section);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task <IActionResult> CreateSectionForCourse(Guid courseId, [FromBody] SectionForCreationDto section)
        {
           /* if (section == null)
            {
                _logger.LogError("SectionForCreationDto object sent from client is null.");
                return BadRequest("SectionForCreationDto object is null");
            }

            //    var organization = _repository.Company.GetCompany(companyId, trackChanges: false);
            var course =await _repository.Course.GetCoursesAsync(courseId, trackChanges: false);

            if (course == null)
            {
                _logger.LogInfo($"User with id: {courseId} doesn't exist in the database.");
                return NotFound();
            } */


            var sectionEntity = _mapper.Map<Section>(section);

            //      _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            _repository.Section.CreateSectionForCourse(courseId, sectionEntity);
           await _repository.SaveAsync();

            var sectionToReturn = _mapper.Map<SectionDto>(sectionEntity);

            return CreatedAtRoute(new { courseId, id = sectionToReturn.Id }, sectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateSectionExistsAttribute))]
        public async Task <IActionResult> DeleteSectionForCourse(Guid courseId, Guid id)
        {
            /*  var course =  await _repository.Course.GetCoursesAsync(courseId, trackChanges: false);
              if ( course==null)
              {
                  _logger.LogInfo($"Course with id: {courseId} doesn't exist in the database.");
                  return NotFound();
              }

              var sectionForCourse =await _repository.Section.GetSectionAsync(courseId, id, trackChanges: false);
              if (sectionForCourse == null)
              {
                  _logger.LogInfo($"Section with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */
            var sectionForCourse = HttpContext.Items["section"] as Section;

            _repository.Section.DeleteSection(sectionForCourse);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateSectionExistsAttribute))]
        public async Task <IActionResult> UpdateSectionForCourse(Guid courseId, Guid id, [FromBody] SectionForUpdateDto section)
        {
            /*  if (section== null)
              {
                  _logger.LogError("SectionForUpdateDto object sent from client is null.");
                  return BadRequest("SectionForUpdateDto object is null");
              }

              if (!ModelState.IsValid)
              {
                  _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                  return UnprocessableEntity(ModelState);
              }


              var course = await _repository.Course.GetCoursesAsync(courseId, trackChanges: false);
              if (course == null)
              {
                  _logger.LogInfo($"Coursewith id: {courseId} doesn't exist in the database.");
                  return NotFound();
              } 

              var sectionEntity =await _repository.Section.GetSectionAsync(courseId, id, trackChanges: true);
              if (sectionEntity == null)
              {
                  _logger.LogInfo($"Section with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

            var sectionEntity = HttpContext.Items["section"] as Section;

            _mapper.Map(section, sectionEntity);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateSectionExistsAttribute))]
        public async Task < IActionResult> PartiallyUpdateSectionForCourse(Guid courseId, Guid id, [FromBody] JsonPatchDocument<SectionForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            /*  var course = await _repository.Course.GetCoursesAsync(courseId, trackChanges: false);
              if (course == null)
              {
                  _logger.LogInfo($"Course with id: {courseId} doesn't exist in the database.");
                  return NotFound();
              }

              var sectionEntity =await _repository.Section.GetSectionAsync(courseId, id, trackChanges: true);
              if (sectionEntity == null)
              {
                  _logger.LogInfo($"Section with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

            var sectionEntity = HttpContext.Items["section"] as Section;

            var sectionToPatch = _mapper.Map<SectionForUpdateDto>(sectionEntity);

            patchDoc.ApplyTo(sectionToPatch, ModelState);

            TryValidateModel(sectionToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            patchDoc.ApplyTo(sectionToPatch);

            _mapper.Map(sectionToPatch, sectionEntity);

          await  _repository.SaveAsync();

            return NoContent();
        }
    }
}
