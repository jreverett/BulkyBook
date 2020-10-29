using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext db;

        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(CoverType coverType)
        {
            var coverTypeFromDb = db.CoverTypes.Find(coverType.Id);

            if (coverTypeFromDb != null)
            {
                db.Entry(coverTypeFromDb).CurrentValues.SetValues(coverType);
            }
        }
    }
}
