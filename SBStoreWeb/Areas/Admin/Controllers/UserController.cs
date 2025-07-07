using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using SBStore.Models.ViewModels;
using SBStore.Utility;
using System.Net.WebSockets;


namespace SBStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _data;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(AppDbContext data,UserManager<IdentityUser> userManager)
        {
            _data = data;
            _userManager = userManager;
           
        }
        public IActionResult Index()
        {
            return View();
        } 

        public IActionResult RoleManagement(string userId)
        {
            //retrieve the role id from user roles table
            string RoleID = _data.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                AppUser = _data.AppUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _data.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _data.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.AppUser.Role = _data.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            return View(RoleVM);
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            //retrieve the role id from user roles table
            string RoleID = _data.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.AppUser.Id).RoleId; //current role

            string oldRole = _data.Roles.FirstOrDefault(u => u.Id == RoleID).Name;

            if (!(roleManagementVM.AppUser.Role == oldRole)) //if current role is not equal to old role
            {
                //a role was updated
                AppUser appUser = _data.AppUsers.FirstOrDefault(u => u.Id == roleManagementVM.AppUser.Id);

                if (roleManagementVM.AppUser.Role == SD.Role_Company)
                {
                    appUser.CompanyId = roleManagementVM.AppUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    appUser.CompanyId = null;
                }
                _data.SaveChanges();
              
                _userManager.RemoveFromRoleAsync(appUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(appUser, roleManagementVM.AppUser.Role).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<AppUser> objUserList = _data.AppUsers.Include(u=>u.Company).ToList();

            var userRoles = _data.UserRoles.ToList();
            var roles = _data.Roles.ToList();
            foreach (var user in objUserList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = objUserList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
           var objFromDb = _data.AppUsers.FirstOrDefault(u => u.Id == id);
            //sprawdzamy czy uzytkownik istnieje w bazie danych, zawsze na poczatku!
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            //jesli uzytkownik istnieje i jego czas zablokowania jest wiekszy od tertaz
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;

            } //if user is already unlocked
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _data.SaveChanges();
            return Json(new { success = true, message = "Operation Successfull" });
        }


        #endregion
    }
}
