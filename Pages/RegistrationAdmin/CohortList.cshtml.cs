using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.RegistrationAdmin {

    public class CohortListModel : PageModel {
        private readonly PermissionsHandler _permissions;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public CohortListModel(PermissionsHandler permissions, RegistrationTestHelper registrationTestHelper) {
            _permissions = permissions;
            _registrationTestHelper = registrationTestHelper;
        }

        public List<RegistrationCohort> Cohorts { get; set; } = default!;

        [BindProperty]
        public RegistrationCohort RegistrationCohort { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            Cohorts = _registrationTestHelper.GetCohorts();
            var id = string.IsNullOrWhiteSpace(Request.Query["id"]) ? 0 : int.Parse(Request.Query["id"]);
            RegistrationCohort = _registrationTestHelper.GetCohort(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            _ = await _registrationTestHelper.SaveCohort(RegistrationCohort);
            return this.RedirectToPage();
        }
    }
}