using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
  public  class SectionForManipulationDto
    {
        [Required(ErrorMessage = "Type is a required field.")]
        [MaxLength(10, ErrorMessage = "Maximum length for the type is 10 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the type is 5 characters.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Start date is a required field.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is a required field.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Created date is a required field.")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Updated date is a required field.")]
        public DateTime UpdatedDate { get; set; }
    }
}
