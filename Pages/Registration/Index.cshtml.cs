using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TqiiLanguageTest.Pages.Registration {

    public class IndexModel : PageModel {
        public bool Exemption { get; set; } = false;

        public string FirstName { get; set; } = "";

        public string LanguageExemption { get; set; } = "";

        public string LastName { get; set; } = "";

        public void OnGet() {
        }
    }
}