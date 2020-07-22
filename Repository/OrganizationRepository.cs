using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task < IEnumerable<Organization>> GetAllOrganizationsAsync(bool trackChanges) =>
         await FindAll(trackChanges)
          .OrderBy(c => c.OrgName)
          .ToListAsync();
         

        public  async Task < Organization >GetOrganizationAsync(Guid OrganizationId, bool trackChanges) =>
           await FindByCondition(c => c.Id.Equals(OrganizationId), trackChanges)
            .SingleOrDefaultAsync();

        public  async Task <IEnumerable<Organization>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
         await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();

        public void CreateOrganization(Organization organization) => Create(organization);

        public void DeleteOrganization(Organization organization)
        {
            Delete(organization);
        }
    }
}
