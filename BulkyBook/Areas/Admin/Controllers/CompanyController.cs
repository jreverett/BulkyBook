using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            if (id == null)
            {
                // Create
                return View(company);
            }

            // Edit
            company = unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = unitOfWork.Company.GetAll();
            return Json(new { data = companies });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    // Create
                    unitOfWork.Company.Add(company);
                }
                else
                {
                    // Edit
                    unitOfWork.Company.update(company);
                }

                unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var company = unitOfWork.Company.Get(id);

            if (company == null)
            {
                return Json(new { success = false, message = $"A company with ID '{id}' could not be found" });
            }

            unitOfWork.Company.Remove(company);
            unitOfWork.Save();

            return Json(new { success = true, message = $"Company '{company.Name}' has been deleted" });
        }

        #endregion
    }
}
