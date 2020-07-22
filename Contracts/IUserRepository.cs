using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRepository
    {


        Task<IEnumerable<User>> GetUsersAsync(Guid organizationId,  bool trackChanges);

  
        Task  <User> GetUserAsync(Guid organizationId, Guid id, bool trackChanges);
        void CreateUserForOrganization(Guid organizationId, User user);

        void CreateUser(User user);
   //   Task  <IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> orgId, bool trackChanges);
        void DeleteUser(User user);

    }
   
}
