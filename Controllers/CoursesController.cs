using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Nest;
using SchoolMgmtAPI.ActionFilters;

namespace SchoolMgmtAPI.Controllers
{
    [Route("api/v1/organizations/{orgId}/users/{userId}/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CoursesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task <IActionResult> GetCoursesForUser(Guid userId)

        {
            var user = await _repository.User.GetUsersAsync(userId,  trackChanges: false);

            if (user == null)
            {
                _logger.LogInfo($"User with id: {userId} doesn't exist in the database.");
                return NotFound();
            }

            var coursesFromDb =await _repository.Course.GetCoursesAsync(userId,  trackChanges: false);

            var coursesDto = _mapper.Map<IEnumerable<CourseDto>>(coursesFromDb);

            return Ok(coursesDto);
        }

       //    [HttpGet("{id}", Name = "GetCourseForUser")]

       [HttpGet("{id}")]
        public async Task <IActionResult> GetCourseForUser(Guid userId, Guid id)
        {
            var user = await _repository.User.GetUsersAsync(userId,  trackChanges: false);
            if (user == null)
            {
                _logger.LogInfo($"User with id: {userId} doesn't exist in the database.");
                return NotFound();
            }

            var courseDb =await _repository.Course.GetCourseAsync(userId, id, trackChanges: false);
            if (courseDb == null)
            {
                _logger.LogInfo($"Course with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var course = _mapper.Map<CourseDto>(courseDb);

            return Ok(course);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task <IActionResult> CreateCourseForUser( Guid userId, [FromBody]
        CourseForCreationDto course)
        {
          /*  if (course == null)
            {
                _logger.LogError("CourseForCreationDto object sent from client is null.");
                return BadRequest("CourseForCreationDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EmployeeForCreationDto object");

                return UnprocessableEntity(ModelState);
            }

            //    var organization = _repository.Company.GetCompany(companyId, trackChanges: false);
            var user = await  _repository.User.GetUsersAsync( userId,  trackChanges: false);

            if (user == null)
            {
                _logger.LogInfo($"User with id: {userId} doesn't exist in the database.");
                return NotFound();
            } */

            var courseEntity = _mapper.Map<Course>(course);

            //      _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            _repository.Course.CreateCourseForUser(userId, courseEntity);
           await _repository.SaveAsync();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);

            return CreatedAtRoute( new { userId, id = courseToReturn.Id }, courseToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCourseExistsAttribute))]
        public async Task <IActionResult> DeleteCourseForUser(Guid userId, Guid id)
        {
            /*  var user = await _repository.User.GetUsersAsync(userId, trackChanges: false);
              if (user == null)
              {
                  _logger.LogInfo($"User with id: {userId} doesn't exist in the database.");
                  return NotFound();
              }

              var courseForUser =await _repository.Course.GetCourseAsync(userId, id, trackChanges: false);
              if (courseForUser == null)
              {
                  _logger.LogInfo($"User with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */
            var courseForUser = HttpContext.Items["course"] as Course;

            _repository.Course.DeleteCourse(courseForUser);

           await _repository.SaveAsync();

            return NoContent();


        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCourseExistsAttribute))]
        public async Task <IActionResult> UpdateCourseForUser(Guid userId, Guid id, [FromBody] CourseForUpdateDto course)
        {
            /*  if (course == null)
              {
                  _logger.LogError("CourseForUpdateDto object sent from client is null.");
                  return BadRequest("CourseForUpdateDto object is null");
              }

              if (!ModelState.IsValid)
              {
                  _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                  return UnprocessableEntity(ModelState);
              }

              var user = await _repository.User.GetUsersAsync(userId, trackChanges: false);
              if (user == null)
              {
                  _logger.LogInfo($"User with id: {userId} doesn't exist in the database.");
                  return NotFound();
              } 

              var courseEntity = await _repository.Course.GetCourseAsync(userId, id, trackChanges: true);
              if (courseEntity == null)
              {
                  _logger.LogInfo($"Course with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

            var courseEntity = HttpContext.Items["Course"] as Course;

            _mapper.Map(course, courseEntity);
          await  _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateCourseExistsAttribute))]
        public async Task <IActionResult> PartiallyUpdateCourseForUser(Guid userId, 
            Guid id, [FromBody] JsonPatchDocument<CourseForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            /*     var user = await _repository.User.GetUsersAsync(userId, trackChanges: false);
                 if (user == null)
                 {
                     _logger.LogInfo($"User with id: {userId} doesn't exist in the database.");
                     return NotFound();
                 }

                 var courseEntity = await _repository.Course.GetCourseAsync(userId, id, trackChanges: true);
                 if (courseEntity == null)
                 {
                     _logger.LogInfo($"Course with id: {id} doesn't exist in the database.");
                     return NotFound();
                 }  */

            var courseEntity = HttpContext.Items["course"] as Course;

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseEntity);

            patchDoc.ApplyTo(courseToPatch, ModelState);

            TryValidateModel(courseToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            //    patchDoc.ApplyTo(courseToPatch);

            _mapper.Map(courseToPatch, courseEntity);

           await _repository.SaveAsync();

            return NoContent();
        }


    }
}
