using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<IdentityUser> userManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = unitOfWork.ApplicationUser.GetAll(null, null, "Company");

            foreach (var user in users)
            {
                user.Role = userManager.GetRolesAsync(user).Result.FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult ToggleLock([FromBody] string id)
        {
            var user = unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == id);
            string response;

            if (user == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // Unlock
                response = "unlocked";
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                // Lock
                response = "locked";
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            unitOfWork.Save();

            return Json(new { success = true, message = $"User {response} successfully" });
        }

        #endregion
    }
}
