using Microsoft.AspNetCore.Mvc;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Utility;
using System.Security.Claims;

namespace SBStoreWeb.ViewComponents
{
    public class  ShoppingCartViewComponent : ViewComponent
    {
        //here we have to go to the shopping cart database and get the shopping cart for a logged in user. In order to do that, just follow along with these steps
        //1. Inject unitOfWork
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //retrieve the userId of the logged-in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null) //that means the user is logged in
            {
                if (HttpContext.Session.GetInt32(SD.SessionCard) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCard,
                    _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == claim.Value).Count());
                }
            
                return View(HttpContext.Session.GetInt32(SD.SessionCard));
            }
            else //if the claim is null, not logged in
            {
                HttpContext.Session.Clear(); //and that way, we do not have to do that in the Logout
                return View(0);
            }
        }
    }
    
    
}
