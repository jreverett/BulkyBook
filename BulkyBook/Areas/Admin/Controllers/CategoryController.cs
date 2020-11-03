using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                // CREATE
                return View(category);
            }

            // EDIT
            category = unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = unitOfWork.Category.GetAll();
            return Json(new { data = categories });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    // Create
                    unitOfWork.Category.Add(category);
                }
                else
                {
                    // Edit
                    unitOfWork.Category.Update(category);
                }

                unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var category = unitOfWork.Category.Get(id);

            if (category == null)
            {
                return Json(new { success = false, message = $"A category with ID '{id}' could not be found" });
            }

            unitOfWork.Category.Remove(category);
            unitOfWork.Save();

            return Json(new { success = true, message = $"Category '{category.Name}' has been deleted" });
        }

        #endregion
    }
}
