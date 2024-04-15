using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;

namespace SBStoreWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            return Json(new { data = objOrderHeaders });
        }
       


        #endregion
    }
}
