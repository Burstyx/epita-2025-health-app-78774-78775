using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages;

[Authorize(Roles = "Patient")]
public class Patient : PageModel
{
    public void OnGet()
    {
        
    }
}