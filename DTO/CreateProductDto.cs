using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcshippers_Task.DTO
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3, ErrorMessage = "Name must be greater than 3 char")]
        public string ProductName { get; set; }
        [Required]
        public double Productprice { get; set; }
        [Required]
        public int ProductQuantity { get; set; }
        [Required]
        public IFormFile? ProductPhoto { get; set; }
   
    

    }
}
