using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.RegistrationAdmin {

    public class CohortEditModel : PageModel {
        private readonly PermissionsHandler _permissions;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public CohortEditModel(PermissionsHandler permissions, RegistrationTestHelper registrationTestHelper) {
            _permissions = permissions;
            _registrationTestHelper = registrationTestHelper;
        }

        public RegistrationCohort Cohort { get; set; } = default!;

        public List<string> Languages { get; set; } = default!;

        [BindProperty]
        public RegistrationTest RegistrationTest { get; set; } = default!;

        public List<RegistrationTest> Tests { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var cohortId = string.IsNullOrWhiteSpace(Request.Query["cohortId"]) ? 0 : int.Parse(Request.Query["cohortId"]);
            Cohort = _registrationTestHelper.GetCohort(cohortId);
            Tests = _registrationTestHelper.GetTests(cohortId);
            Languages = _registrationTestHelper.GetLanguages();
            var id = string.IsNullOrWhiteSpace(Request.Query["id"]) ? 0 : int.Parse(Request.Query["id"]);
            RegistrationTest = _registrationTestHelper.GetTest(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            RegistrationTest.RegistrationCohortId = int.Parse(Request.Query["cohortId"]);
            _ = await _registrationTestHelper.SaveTest(RegistrationTest);
            return this.RedirectToPage(new { cohortId = Request.Query["cohortId"] });
        }
    }
}