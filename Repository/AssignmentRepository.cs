using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Repository
{
    class AssignmentRepository : RepositoryBase<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(RepositoryContext repositoryContext)
             : base(repositoryContext)
        {
        }



        public async Task<Assignment> GetAssignmentAsync(Guid enrollmentId, Guid id, bool trackChanges)
     => await FindByCondition(e => e.EnrollmentId.Equals(enrollmentId) && e.Id.Equals(id), trackChanges)
.SingleOrDefaultAsync();

        public async Task<IEnumerable<Assignment>> GetAssignmentsAsync(Guid enrollmentId, bool trackChanges) =>
await FindByCondition(e => e.EnrollmentId.Equals(enrollmentId), trackChanges)
.OrderBy(e => e.EnrollmentId).ToListAsync();



        public void CreateAssignmentForEnrollment(Guid enrollmentId, Assignment assignment)
        {
            assignment.EnrollmentId = enrollmentId;
            Create(assignment);
        }

        public void DeleteAssignment(Assignment assignment)
        {
            Delete(assignment);
        }




    }


}

