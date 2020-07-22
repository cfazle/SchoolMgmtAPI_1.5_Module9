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
    class SectionRepository : RepositoryBase<Section>, ISectionRepository
    {
        public SectionRepository(RepositoryContext repositoryContext)
             : base(repositoryContext)
        {
        }



        public async Task<Section> GetSectionAsync(Guid courseId, Guid id, bool trackChanges)
     => await FindByCondition(e => e.CourseId.Equals(courseId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();


        public async Task<IEnumerable<Section>> GetSectionsAsync(Guid courseId, bool trackChanges) =>
      await FindByCondition(e => e.CourseId.Equals(courseId), trackChanges)
      .OrderBy(e => e.CourseId).ToListAsync();


        public void CreateSectionForCourse(Guid courseId, Section section)
        {
            section.CourseId = courseId;
            Create(section);
        }

        public void DeleteSection(Section section)
        {
            Delete(section);
        }

       

      
    }
}
   
