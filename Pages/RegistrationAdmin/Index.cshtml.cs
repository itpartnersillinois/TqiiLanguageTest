using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages.RegistrationAdmin {

    public class IndexModel : PageModel {
        private readonly PermissionsHandler _permissions;

        public IndexModel(PermissionsHandler permissions) {
            _permissions = permissions;
        }

        public IActionResult OnGet() => !_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")
                ? Unauthorized()
                : Page();
    }
}