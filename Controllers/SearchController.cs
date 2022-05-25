using Mcshippers_Task.DTO;
using Mcshippers_Task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mcshippers_Task.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicatioDbContext _context;


        public SearchController(ApplicatioDbContext context)
        {
            _context = context;
        }

        [HttpGet ("{search}")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchAsync(string searchKey)
        {
            IQueryable<Product> MatcheResults = _context.Products;
            string x = "123456789";
            if (x.Contains(searchKey))
            {
                double Price = double.Parse(searchKey);
                MatcheResults = MatcheResults.Where(x => x.price==Price);
            }
           
           
           else if (searchKey.Length>0)
            {

                MatcheResults = MatcheResults.Where(x => x.Name.Contains(searchKey));
            }
            return await MatcheResults.ToListAsync();
        }

    }
}
