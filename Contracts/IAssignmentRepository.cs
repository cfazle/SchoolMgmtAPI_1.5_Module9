using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
   public  interface IAssignmentRepository
    {
        Task <IEnumerable<Assignment>> GetAssignmentsAsync(Guid enrollmentId, bool trackChanges);
        Task <Assignment> GetAssignmentAsync(Guid enrollmentId, Guid id, bool trackChanges);

        void CreateAssignmentForEnrollment(Guid enrollmentId, Assignment assignment);
        void DeleteAssignment(Assignment assignment);
    }
}
