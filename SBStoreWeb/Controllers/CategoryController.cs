using Microsoft.AspNetCore.Mvc;

namespace SBStoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
