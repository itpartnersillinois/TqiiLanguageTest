using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TqiiLanguageTest.Pages {

    [AllowAnonymous]
    public class HelpModel : PageModel {

        public void OnGet() {
        }
    }
}