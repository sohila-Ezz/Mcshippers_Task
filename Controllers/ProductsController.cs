using Mcshippers_Task.DTO;
using Mcshippers_Task.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Mcshippers_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicatioDbContext _context;
        private new List<string> allowExxtention = new List<string> { ".png", ".jpg" };

        public ProductsController(ApplicatioDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProductesAsync()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetOneProductByIDAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"No Product was found with ID {id}");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProductAsync([FromForm] CreateProductDto pro)
        {
            if(pro.ProductPhoto == null) return BadRequest("Image Is Required ");
            if (!allowExxtention.Contains(Path.GetExtension(pro.ProductPhoto.FileName).ToLower()))
                return BadRequest("Onl .png or .jpg images are allow ");
  
            using var dataStream = new MemoryStream();
            await pro.ProductPhoto.CopyToAsync(dataStream);
            var product = new Product
            {

                Name = pro.ProductName,
                price = pro.Productprice,
                Quantity = pro.ProductQuantity,
                Image = dataStream.ToArray()
            };
            await _context.AddAsync(product);
            _context.SaveChanges();

            return Ok(product);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromForm] CreateProductDto product)
        {
            var pro = await _context.Products.FindAsync(id);
            if (pro == null)
                return NotFound($"No Product was found with ID {id}");
           
            if (product.ProductPhoto != null)
            {
                if (!allowExxtention.Contains(Path.GetExtension(product.ProductPhoto.FileName).ToLower()))
                    return BadRequest("Onl .png or .jpg images are allow ");
                using var dataStream = new MemoryStream();
                await product.ProductPhoto.CopyToAsync(dataStream);
                pro.Image = dataStream.ToArray();
            }
            pro.Name = product.ProductName;
            pro.price = product.Productprice;
            pro.Quantity = product.ProductQuantity;
 
            await _context.SaveChangesAsync();
            return Ok(pro);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

    }
}
