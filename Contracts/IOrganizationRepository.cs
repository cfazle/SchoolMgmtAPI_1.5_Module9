using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
   public  interface IOrganizationRepository
    {
        Task <IEnumerable<Organization>> GetAllOrganizationsAsync(bool trackChanges);
        Task<Organization>GetOrganizationAsync(Guid OrganizationId, bool trackChanges);
        void CreateOrganization(Organization organization);
        Task <IEnumerable<Organization>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        void DeleteOrganization(Organization organization);
    }
}
