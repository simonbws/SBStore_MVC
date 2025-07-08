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
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
           
        }
        public IActionResult Index()
        {
            return View();
        } 

        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                AppUser = _unitOfWork.AppUser.Get(u => u.Id == userId, includeProperties:"Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.AppUser.Role = _userManager.GetRolesAsync(_unitOfWork.AppUser.Get(u => u.Id == userId))
                .GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string oldRole  = _userManager.GetRolesAsync(_unitOfWork.AppUser.Get(u => u.Id == roleManagementVM.AppUser.Id))
                .GetAwaiter().GetResult().FirstOrDefault();


            AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == roleManagementVM.AppUser.Id);

            if (!(roleManagementVM.AppUser.Role == oldRole)) //if current role is not equal to old role
            {
                //a role was updated
                if (roleManagementVM.AppUser.Role == SD.Role_Company)
                {
                    appUser.CompanyId = roleManagementVM.AppUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    appUser.CompanyId = null;
                }
                _unitOfWork.AppUser.Update(appUser);
                _unitOfWork.Save();
              
                _userManager.RemoveFromRoleAsync(appUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(appUser, roleManagementVM.AppUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if(oldRole == SD.Role_Company && appUser.CompanyId != roleManagementVM.AppUser.CompanyId)
                {
                    appUser.CompanyId = roleManagementVM.AppUser.CompanyId;
                    _unitOfWork.AppUser.Update(appUser);
                    _unitOfWork.Save();

                }
            }
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<AppUser> objUserList = _unitOfWork.AppUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if (user.Company == null)
                {
                    user.Company = new Company() { Name = "" };
                }
            }

            return Json(new { data = objUserList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
           var objFromDb = _unitOfWork.AppUser.Get(u => u.Id == id);
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
            _unitOfWork.AppUser.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successfull" });
        }


        #endregion
    }
}
