﻿using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

using Entities.Models;
using Entities.DataTransferObjects;
using SchoolMgmtAPI.ModelBinders;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using SchoolMgmtAPI.ActionFilters;

namespace SchoolMgmtAPI.Controllers
{
    [Route("api/v1/organizations")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;


        public OrganizationController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task <IActionResult> GetOrganizations()
        {

            var organizations = await _repository.Organization.GetAllOrganizationsAsync(trackChanges: false);
            //  return Ok(organizations);
            var organizationDto = _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
            return Ok(organizationDto);

        }

        [HttpGet("{id}", Name = "OrganizationById")]

        public async Task <IActionResult> GetOrganization(Guid id)
        {

            var organization = await _repository.Organization.GetOrganizationAsync(id, trackChanges: false);
            if (organization == null)
            {
                _logger.LogInfo($"Organization with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var organizationDto = _mapper.Map<OrganizationDto>(organization);
                return Ok(organizationDto);
            }


        }

        [HttpGet("collection/({ids})", Name = "OrganizationCollection")]
        public async Task <IActionResult> GetOrganizationCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]
        IEnumerable<Guid> ids)
        //  public IActionResult GetOrganizationCollection(IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var organizationEntities =await _repository.Organization.GetByIdsAsync(ids, trackChanges: false);

            if (ids.Count() != organizationEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }

            var organizationsToReturn = _mapper.Map<IEnumerable<OrganizationDto>>(organizationEntities);
            return Ok(organizationsToReturn);
        }


        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]

        public async Task <IActionResult> CreateOrganization([FromBody] OrganizationForCreationDto organization)
        {
          /*  if (organization == null)
            {
                _logger.LogError("CompanyForCreationDto object sent from client is null.");

                return BadRequest("CompanyForCreationDto object is null");
            }
          */

            var organizationEntity =  _mapper.Map<Organization>(organization);

            _repository.Organization.CreateOrganization(organizationEntity);

           await _repository.SaveAsync();

            var organizationToReturn = _mapper.Map<OrganizationDto>(organizationEntity);

            return CreatedAtRoute("OrganizationById",

                new { id = organizationToReturn.Id }, organizationToReturn);
        }

        [HttpPost("collection")]
        public async Task < IActionResult> CreateOrganizationCollection([FromBody]
        IEnumerable<OrganizationForCreationDto> organizationCollection)
        {
           if (organizationCollection == null)
            {
                _logger.LogError("Organization collection sent from client is null.");
                return BadRequest("Organization collection is null");
            }

            var organizationEntities = _mapper.Map<IEnumerable<Organization>>(organizationCollection);
            foreach (var organization in organizationEntities)
            {
                _repository.Organization.CreateOrganization(organization);
            }

           await _repository.SaveAsync();

            var organizationCollectionToReturn = _mapper.Map<IEnumerable<OrganizationDto>>
                (organizationEntities);
            var ids = string.Join(",", organizationCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("OrganizationCollection", new { ids }, organizationCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateOrganizationExistsAttribute))]
        public async Task < IActionResult> DeleteOrganization(Guid id)
        {
            var organization = await _repository.Organization.GetOrganizationAsync(id, trackChanges: false);
            if (organization == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _repository.Organization.DeleteOrganization(organization);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateOrganizationExistsAttribute))]
        public async Task <IActionResult> UpdateOrganization(Guid id, [FromBody] OrganizationForUpdateDto organization)
        {
            /*   if (organization == null)
               {
                   _logger.LogError("OrganizationForUpdateDto object sent from client is null.");
                   return BadRequest("OrganizationForUpdateDto object is null");
               }

               if (!ModelState.IsValid) {
                   _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                   return UnprocessableEntity(ModelState);
               } */

            var organizationEntity = HttpContext.Items["organization"] as Organization;

       /*     var organizationEntity = await _repository.Organization.GetOrganizationAsync(id, trackChanges: true);
            if (organizationEntity == null)
            {
                _logger.LogInfo($"Organization with id: {id} doesn't exist in the database.");
                return NotFound();
            } */

            _mapper.Map(organization, organizationEntity);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task <IActionResult> PartiallyUpdateOrganization( Guid id, [FromBody] JsonPatchDocument<OrganizationForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

        /*    var organization = _repository.Organization.GetOrganization(Id, trackChanges: false);
            if (organization == null)
            {
                _logger.LogInfo($"Company with id: {orgId} doesn't exist in the database.");
                return NotFound();
            } */

            var organizationEntity = await _repository.Organization.GetOrganizationAsync( id, trackChanges: true);
            if (organizationEntity == null)
            {
                _logger.LogInfo($"Organization with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var organizationToPatch = _mapper.Map<OrganizationForUpdateDto>(organizationEntity);

            patchDoc.ApplyTo(organizationToPatch, ModelState);

            TryValidateModel(organizationToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(organizationToPatch, organizationEntity);

            await _repository.SaveAsync();

            return NoContent();
        }

    }
}
