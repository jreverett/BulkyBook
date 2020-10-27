using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if (id == null)
            {
                // CREATE
                return View(coverType);
            }

            // EDIT
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            coverType = unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameters);
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypes = unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = coverTypes });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", coverType.Name);

                if (coverType.Id == 0)
                {
                    // Create
                    unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameters);
                }
                else
                {
                    // Edit
                    parameters.Add("@Id", coverType.Id);
                    unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameters);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var coverType = unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameters);

            if (coverType == null)
            {
                return Json(new { success = false, message = $"A cover type with ID '{id}' could not be found" });
            }

            unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameters);

            return Json(new { success = true, message = $"Cover type '{coverType.Name}' has been deleted" });
        }

        #endregion
    }
}
