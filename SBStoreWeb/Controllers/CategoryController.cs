using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using SBStoreWeb.Data;
using SBStoreWeb.Models;
using System.Net.WebSockets;

namespace SBStoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _data;
        public CategoryController(AppDbContext data)
        {
            _data = data;
        }
        public IActionResult Index()
        {
            List<Category> objCatList = _data.Categories.ToList();
            return View(objCatList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder must be different from the Name");
            }

            if (ModelState.IsValid)
            {
                _data.Categories.Add(obj);
                _data.SaveChanges();
                TempData["success"] = "Category has beean created successfully";
                return RedirectToAction("Index", "Category");
            }
           return View();
            
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id==0) 
            {
                return NotFound();
            }
            Category? categoryFromData = _data.Categories.Find(id);
            //Category? categoryFromData1 = _data.Categories.FirstOrDefault(u=>u.Id==id);
            //Category? categoryFromData2 = _data.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromData == null)
            {
                return NotFound();
            }

            return View(categoryFromData);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _data.Categories.Update(obj);
                _data.SaveChanges();
                TempData["success"] = "Category has beean updated successfully";
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
            Category? categoryFromData = _data.Categories.Find(id);
            //Category? categoryFromData1 = _data.Categories.FirstOrDefault(u=>u.Id==id);
            //Category? categoryFromData2 = _data.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromData == null)
            {
                return NotFound();
            }

            return View(categoryFromData);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _data.Categories.Find(id);
            if(obj == null)
            {
                return NotFound();
            }
            _data.Categories.Remove(obj);
            _data.SaveChanges();
            TempData["success"] = "Category has deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
