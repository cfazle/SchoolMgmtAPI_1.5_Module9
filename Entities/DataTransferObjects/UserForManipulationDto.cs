using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class UserForManipulationDto
    {
        [Required(ErrorMessage = "User name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        [MinLength(5, ErrorMessage = "Minimum length for the Name is 5 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is a required field.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Created Date is a required field.")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Updated Date is a required field.")]
        public DateTime UpdatedDate { get; set; }

        //    public IEnumerable<CourseForCreationDto> courses { get; set; }
        //  User t parent = _context.Parent.Include(p => p.Children.Select(c => c.Grandchild).FirstOrDefault();
    }
}

