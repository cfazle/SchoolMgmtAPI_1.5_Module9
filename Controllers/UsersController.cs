using System;
using System.Collections.Generic;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Entities.Models;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SchoolMgmtAPI.ActionFilters;

namespace SchoolMgmtAPI.Controllers
{
    [Route("api/v1/organizations/{orgId}/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public UsersController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task <IActionResult> GetUsersForOrganization(Guid orgId)

        {
            var organization = await _repository.Organization.GetOrganizationAsync(orgId, trackChanges: false);

            if (organization == null)
            {
                _logger.LogInfo($"Organization with id: {orgId} doesn't exist in the database.");
                return NotFound();
            }

            var usersFromDb =await _repository.User.GetUsersAsync(orgId, trackChanges: false);

            var usersDto =_mapper.Map<IEnumerable<UserDto>>(usersFromDb);

            return Ok(usersDto);
        }

         [HttpGet("{id}", Name = "GetUserForOrganization")]
       // [HttpGet("{id}")]
        public async Task <IActionResult> GetUserForOrganization(Guid orgId, Guid id)
        {
            var organization = await _repository.Organization.GetOrganizationAsync(orgId, trackChanges: false);
            if (organization == null)
            {
                _logger.LogInfo($"User with id: {orgId} doesn't exist in the database.");
                return NotFound();
            }

            var userDb =await  _repository.User.GetUserAsync(orgId, id, trackChanges: false);
            if (userDb == null)
            {
                _logger.LogInfo($"user with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(userDb);

            return Ok(user);
        }


        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task <IActionResult> CreateUserForOrganization(Guid orgId, [FromBody] UserForCreationDto user)
        {
            /*   if (user== null)
               {
                   _logger.LogError("UserForCreationDto object sent from client is null.");
                   return BadRequest("UserForCreationDto object is null");
               }

               if (!ModelState.IsValid)
               {
                   _logger.LogError("Invalid model state for the EmployeeForCreationDto object");

                   return UnprocessableEntity(ModelState);
               } */


            //    var organization = _repository.Company.GetCompany(companyId, trackChanges: false);
            /*   var organization =await _repository.Organization.GetOrganizationAsync(orgId, trackChanges: false);
               if (organization == null)
               {
                   _logger.LogInfo($"Organization with id: {orgId} doesn't exist in the database.");
                   return NotFound();
               } */
        //    var userEntity = HttpContext.Items["user"] as User;

            var userEntity = _mapper.Map<User>(user);

            //      _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            _repository.User.CreateUserForOrganization(orgId, userEntity);
           await _repository.SaveAsync();

            var userToReturn = _mapper.Map<UserDto>(userEntity);

            return CreatedAtRoute("GetUserForOrganization", new { orgId, id = userToReturn.Id }, userToReturn);
         //   return CreatedAtRoute( new { orgId, id = userToReturn.Id }, userToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateUserExistsAttribute))]
        public async Task <IActionResult> DeleteUserorOrganization(Guid orgId, Guid id)
        {
            /* var organization = await _repository.Organization.GetOrganizationAsync(orgId, trackChanges: false);
             if (organization == null)
             {
                 _logger.LogInfo($"Organization with id: {orgId} doesn't exist in the database.");
                 return NotFound();
             }

             var userForOrganization =await  _repository.User.GetUserAsync(orgId, id, trackChanges: false);
             if (userForOrganization == null)
             {
                 _logger.LogInfo($"User with id: {id} doesn't exist in the database.");
                 return NotFound();
             } */

            var userForOrganization = HttpContext.Items["user"] as User;

            _repository.User.DeleteUser(userForOrganization);

           await  _repository.SaveAsync();

            return NoContent();
        }
       
        
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateUserExistsAttribute))]
        public async Task< IActionResult> UpdateUserForOrganization(Guid orgId, Guid id, [FromBody] UserForUpdateDto user)
        {
            /*  if (user == null)
              {
                  _logger.LogError("UserForUpdateDto object sent from client is null.");
                  return BadRequest("UserForUpdateDto object is null");
              }

              if (!ModelState.IsValid)
              {
                  _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                  return UnprocessableEntity(ModelState);
              }


              var organization = await _repository.Organization.GetOrganizationAsync(orgId, trackChanges: false);
              if (organization == null)
              {
                  _logger.LogInfo($"Organization with id: {orgId} doesn't exist in the database.");
                  return NotFound();
              } 

              var userEntity = await _repository.User.GetUserAsync(orgId, id, trackChanges: true);
              if (userEntity == null)
              {
                  _logger.LogInfo($"User with id: {id} doesn't exist in the database.");
                  return NotFound();
              } */

           

            var userEntity = HttpContext.Items["user"] as User;

            _mapper.Map(user, userEntity);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateUserExistsAttribute))]
        public async Task < IActionResult> PartiallyUpdateUserForOrganization(Guid orgId, Guid id, [FromBody] JsonPatchDocument<UserForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            /*  var organization = await _repository.Organization.GetOrganizationAsync(orgId, trackChanges: false);
              if (organization == null)
              {
                  _logger.LogInfo($"Company with id: {orgId} doesn't exist in the database.");
                  return NotFound();
              } */

            /*   var userEntity = await _repository.User.GetUserAsync(orgId, id, trackChanges: true);
               if (userEntity == null)
               {
                   _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                   return NotFound();
               } */

            var userEntity = HttpContext.Items["user"] as User;
            var userToPatch = _mapper.Map<UserForUpdateDto>(userEntity);

            patchDoc.ApplyTo(userToPatch, ModelState);

            TryValidateModel(userToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(userToPatch, userEntity);

          await  _repository.SaveAsync();

            return NoContent();
        }
    }
}
    

