using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Repository
{

    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }



      
        public async Task<User> GetUserAsync(Guid orgId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.OrganizationId.Equals(orgId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

  
           public async Task<IEnumerable<User>> GetUsersAsync(Guid orgId, bool trackChanges) =>
            await FindByCondition(e => e.OrganizationId.Equals(orgId), trackChanges)
             .OrderBy(e => e.UserName).ToListAsync();

        public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
           await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();

        public void CreateUserForOrganization(Guid orgId, User user)
        {
            user.OrganizationId = orgId;
            Create(user);
        }
     

        public void DeleteUser(User user)
        {
            Delete(user);
        }

        public void CreateUser(User user) => Create(user);

       

       
    }
}
