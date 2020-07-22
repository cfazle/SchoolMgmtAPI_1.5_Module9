using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
  public  class CourseForManipulationDto 
    {
        [Required(ErrorMessage = "Course name is a required field.")]
        [MaxLength(10, ErrorMessage = "Maximum length for the course is 10 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the course is 5 characters.")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Created date is a required field.")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Updated date is a required field.")]
        public DateTime UpdatedDate { get; set; }

        public IEnumerable<SectionForCreationDto> sections { get; set; }
    }
}
