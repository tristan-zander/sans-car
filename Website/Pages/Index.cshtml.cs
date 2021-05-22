using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SansCar.Pages
{
    public class Index : PageModel
    {
        public string Github { get; set; } = "https://github.com/tristan-zander";
        
        public void OnGet()
        {
        }
    }
}