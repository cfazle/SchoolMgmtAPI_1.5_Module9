using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICourseRepository
    {
        Task <IEnumerable<Course>> GetCoursesAsync(Guid userId,  bool trackChanges);
        Task <Course> GetCourseAsync(Guid userId, Guid id, bool trackChanges);

        void CreateCourseForUser(Guid userId,Course course);
        void DeleteCourse(Course course);
    }
    
}
