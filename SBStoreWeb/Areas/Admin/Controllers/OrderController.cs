using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using SBStore.Models.ViewModels;
using SBStore.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace SBStoreWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "AppUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;

            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;

            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details has been updated!";
            return RedirectToAction(nameof(Details), new {orderId = orderHeaderFromDb.Id});
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details has been updated!";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            //update properties
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            if(orderHeader.PaymentStatus==SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Success!";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;

            //If user is admin and employee get all the orders from everyone
            if(User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee)) {
                 objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            }
            else
            //if not, others we will be able to see only their own orders, so we will filter that based on userId
            //First, get the userId
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                //load that obj
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.AppUserId == userId, includeProperties:"AppUser");

            }
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = objOrderHeaders });
        }
       


        #endregion
    }
}
