using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

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
            var categoryFromDb = db.Categories.Find(category.Id);

            if (categoryFromDb != null)
            {
                db.Entry(categoryFromDb).CurrentValues.SetValues(category);
            }
        }
    }
}
