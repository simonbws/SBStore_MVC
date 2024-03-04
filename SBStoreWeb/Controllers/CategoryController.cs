using Microsoft.AspNetCore.Mvc;
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
    }
}
