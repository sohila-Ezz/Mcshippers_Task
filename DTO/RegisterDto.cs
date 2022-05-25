using System.ComponentModel.DataAnnotations;

namespace Mcshippers_Task.DTO
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
       
        public string Password { get; set; }

        [Required]
       
        public string ConfirmPassword { get; set; }

    }
}
