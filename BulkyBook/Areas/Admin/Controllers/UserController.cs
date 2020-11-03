using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        #endregion
    }
}
