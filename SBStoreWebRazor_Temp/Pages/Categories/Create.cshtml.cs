using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SBStoreWebRazor_Temp.Data;
using SBStoreWebRazor_Temp.Models;

namespace SBStoreWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _data;
        public Category Category { get; set; }
        public CreateModel(AppDbContext data)
        {
            _data = data;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _data.Categories.Add(Category);
            _data.SaveChanges();
            TempData["success"] = "Category has beean created successfully";
            return RedirectToPage("Index");
        }
    }
}
