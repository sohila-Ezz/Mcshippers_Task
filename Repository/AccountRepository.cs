using Mcshippers_Task.Models;
using System.Collections.Generic;

namespace Mcshippers_Task.Repository
{
    public class AccountRepository : IAccountRepository
    {
        ApplicatioDbContext context;
        public AccountRepository(ApplicatioDbContext _context)
        {
            context = _context;
        }

        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<ApplicationUser> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public ApplicationUser GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public int Insert(ApplicationUser item)
        {
            throw new System.NotImplementedException();
        }

        public int Update(int id, ApplicationUser item)
        {
            throw new System.NotImplementedException();
        }
    }
}
