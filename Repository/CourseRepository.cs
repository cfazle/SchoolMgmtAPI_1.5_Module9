using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class CourseRepository : RepositoryBase<Course>, ICourseRepository
    {
        public CourseRepository(RepositoryContext repositoryContext)
             : base(repositoryContext)
        {
        }






        public async Task<Course> GetCourseAsync(Guid userId, Guid id, bool trackChanges)
        => await FindByCondition(e => e.UserId.Equals(userId) && e.Id.Equals(id), trackChanges)
               .SingleOrDefaultAsync();

        public async Task<IEnumerable<Course>> GetCoursesAsync(Guid userId,  bool trackChanges) =>
            await FindByCondition(e => e.UserId.Equals(userId), trackChanges)
       .OrderBy(e => e.CourseName)
            .ToListAsync();



        public void CreateCourseForUser(Guid userId, Course course)
        {
            course.UserId = userId;
            Create(course);
        }

        public void DeleteCourse(Course course)
        {
            Delete(course);
        }

    }   
}
