using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System.Linq;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Category category)
        {
            var categoryFromDb = db.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (categoryFromDb != null)
            {
                categoryFromDb.Name = category.Name;
            }
        }
    }
}
