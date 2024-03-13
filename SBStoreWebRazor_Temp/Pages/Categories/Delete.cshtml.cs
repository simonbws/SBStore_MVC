using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SBStoreWebRazor_Temp.Data;
using SBStoreWebRazor_Temp.Models;

namespace SBStoreWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _data;
        public Category Category { get; set; }
        public DeleteModel(AppDbContext data)
        {
            _data = data;
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                Category = _data.Categories.Find(id);
            }
        }

        public IActionResult OnPost()
        {
            Category? obj = _data.Categories.Find(Category.Id);
            if (obj == null)
            {
                return NotFound();
            }
            _data.Categories.Remove(obj);
            _data.SaveChanges();
            TempData["success"] = "Category has beean deleted successfully";
            return RedirectToPage("Index");
        }
    }
}
