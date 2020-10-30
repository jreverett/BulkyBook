using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void update(Company company)
        {
            var companyFromDb = db.Companies.Find(company.Id);

            if (companyFromDb != null)
            {
                db.Entry(companyFromDb).CurrentValues.SetValues(company);
            }
        }
    }
}
