using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
   public class AssignmentForManipulationDto
    {

        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the title is 30 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the title is 5 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the description is 30 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the description is 5 characters.")]
        public string Description { get; set; }
    }
}
