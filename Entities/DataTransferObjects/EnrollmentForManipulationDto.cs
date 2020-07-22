using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
   public class EnrollmentForManipulationDto
    {
        [Required(ErrorMessage = "Attribute name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the attribute name is 30 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the attribute name is 5 characters.")]
        public string AttributeName { get; set; }

        [Required(ErrorMessage = "Start date is a required field.")]
        [DataType(DataType.DateTime)]
      
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is a required field.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Created date is a required field.")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Updated date is a required field.")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; }
    }
}
