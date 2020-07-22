using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnrollmentRepository
    {
        Task <IEnumerable<Enrollment>> GetEnrollmentsAsync(Guid sectionId, bool trackChanges);
        Task <Enrollment> GetEnrollmentAsync(Guid sectionId, Guid id, bool trackChanges);

        void CreateEnrollmentForSection(Guid sectionId, Enrollment Enrollment);
        void DeleteEnrollment(Enrollment enrollment);
    }
}
