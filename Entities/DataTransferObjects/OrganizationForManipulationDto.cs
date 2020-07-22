using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
   public class OrganizationForManipulationDto
    {
        [Required(ErrorMessage = "Org name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the Name is 5 characters.")]
        public string OrgName { get; set; }

        [Required(ErrorMessage = "Address is a required field.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Country is a required field.")]
        public string Country { get; set; }

        public IEnumerable<UserForCreationDto> users { get; set; }
        //   public IEnumerable<CourseForCreationDto> courses { get; set; }
    }
}

