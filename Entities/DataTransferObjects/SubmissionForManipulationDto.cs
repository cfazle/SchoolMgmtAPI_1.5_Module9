using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class SubmissionForManipulationDto
    {
        [Required(ErrorMessage = " Submission title is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the submission title is 30 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the submission title is 5 characters.")]
        public String SubmissionTitle { get; set; }

        [Range(10, int.MaxValue, ErrorMessage = "Score is a required field. It can't be lower than 10")]
        public int Score { get; set; }

        [Required(ErrorMessage = "Created date is a required field.")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Updated date is a required field.")]
        public DateTime UpdatedDate { get; set; }
    }
}
