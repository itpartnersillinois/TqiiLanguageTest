using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages.Admin {

    public class IndexModel : PageModel {
        private readonly PermissionsHandler _permissions;

        public IndexModel(PermissionsHandler permissions) {
            _permissions = permissions;
        }

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            return Page();
        }
    }
}