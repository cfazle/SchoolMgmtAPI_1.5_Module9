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
    class EnrollmentRepository : RepositoryBase<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }



        public async Task<Enrollment> GetEnrollmentAsync(Guid sectionId, Guid id, bool trackChanges)
=> await FindByCondition(e => e.SectionId.Equals(sectionId) && e.Id.Equals(id), trackChanges)
       .SingleOrDefaultAsync();



        public async Task<IEnumerable<Enrollment>> GetEnrollmentsAsync(Guid sectionId, bool trackChanges) =>
 await FindByCondition(e => e.SectionId.Equals(sectionId), trackChanges)
 .OrderBy(e => e.SectionId).ToListAsync();




        public void CreateEnrollmentForSection(Guid sectionId, Enrollment enrollment)
        {
            enrollment.SectionId = sectionId;
            Create(enrollment);
        }

        public void DeleteEnrollment(Enrollment enrollment)
        {
           Delete(enrollment);
        }

       
    }
}
   

