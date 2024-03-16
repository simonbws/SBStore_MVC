using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using System.Net.WebSockets;

namespace SBStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objCatList = _unitOfWork.Product.GetAll().ToList();
            return View(objCatList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj)
        {  

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product has beean created successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromData = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? productFromData1 = _data.Categories.FirstOrDefault(u=>u.Id==id);
            //Product? productFromData2 = _data.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (productFromData == null)
            {
                return NotFound();
            }

            return View(productFromData);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product has beean updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromData = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? productFromData1 = _data.Categories.FirstOrDefault(u=>u.Id==id);
            //Product? productFromData2 = _data.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (productFromData == null)
            {
                return NotFound();
            }

            return View(productFromData);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product has deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
