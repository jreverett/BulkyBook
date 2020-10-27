using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
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
            coverType = unitOfWork.CoverType.Get(id.GetValueOrDefault());
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
            var coverTypes = unitOfWork.CoverType.GetAll();
            return Json(new { data = coverTypes });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    // Create
                    unitOfWork.CoverType.Add(coverType);
                }
                else
                {
                    // Edit
                    unitOfWork.CoverType.Update(coverType);
                }

                unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverType = unitOfWork.CoverType.Get(id);

            if (coverType == null)
            {
                return Json(new { success = false, message = $"A cover type with ID '{id}' could not be found" });
            }

            unitOfWork.CoverType.Remove(coverType);
            unitOfWork.Save();

            return Json(new { success = true, message = $"Cover type '{coverType.Name}' has been deleted" });
        }

        #endregion
    }
}
