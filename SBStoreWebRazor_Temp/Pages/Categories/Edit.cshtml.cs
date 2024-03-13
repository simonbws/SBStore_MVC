using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SBStoreWebRazor_Temp.Data;
using SBStoreWebRazor_Temp.Models;

namespace SBStoreWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _data;
        public Category Category { get; set; }
        public EditModel(AppDbContext data)
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
            if (ModelState.IsValid)
            {
                _data.Categories.Update(Category);
                _data.SaveChanges();
                TempData["success"] = "Category has beean updated successfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
