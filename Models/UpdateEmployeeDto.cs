using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class UpdateEmployeeDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        public string? Department { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than zero.")]
        public decimal Salary { get; set; }
    }
}
