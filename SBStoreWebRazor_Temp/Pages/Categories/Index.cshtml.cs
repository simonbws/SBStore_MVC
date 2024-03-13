using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SBStoreWebRazor_Temp.Data;
using SBStoreWebRazor_Temp.Models;

namespace SBStoreWebRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _data;
        public List<Category> CategoryList { get; set; }
        public IndexModel(AppDbContext data)
        {
                _data = data;
        }
        public void OnGet()
        {
            CategoryList = _data.Categories.ToList();
        }
    }
}
