using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Email;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.RegistrationAdmin {

    [IgnoreAntiforgeryToken(Order = 1001)]
    public class PeopleListModel : PageModel {
        private readonly PermissionsHandler _permissions;
        private readonly RegistrationEmail _registrationEmail;
        private readonly RegistrationPersonHelper _registrationPersonHelper;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public PeopleListModel(PermissionsHandler permissions, RegistrationTestHelper registrationTestHelper, RegistrationPersonHelper registrationPersonHelper, RegistrationEmail registrationEmail) {
            _permissions = permissions;
            _registrationTestHelper = registrationTestHelper;
            _registrationPersonHelper = registrationPersonHelper;
            _registrationEmail = registrationEmail;
        }

        public RegistrationCohort Cohort { get; set; } = default!;
        public List<RegistrationCohortPerson> CohortPeople { get; set; } = default!;

        public List<RegistrationTestPerson> TestPeople { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var id = string.IsNullOrWhiteSpace(Request.Query["cohortid"]) ? 0 : int.Parse(Request.Query["cohortid"]);
            Cohort = _registrationTestHelper.GetCohort(id);
            CohortPeople = _registrationTestHelper.GetCohortPeople(id);
            TestPeople = _registrationTestHelper.GetTestPeople(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var id = string.IsNullOrWhiteSpace(Request.Form["id"]) ? 0 : int.Parse(Request.Form["id"]);
            var formType = string.IsNullOrWhiteSpace(Request.Form["formtype"]) ? "" : Request.Form["formtype"].ToString();
            if (formType.ToLowerInvariant() == "decision") {
                var test = _registrationPersonHelper.GetCohortPerson(id);
                test.IsApproved = Request.Form["value"].ToString() == "approved";
                test.IsDenied = Request.Form["value"].ToString() == "denied";
                test.IsWaitlisted = Request.Form["value"].ToString() == "wait";
                test.DateRegistered = null;
                test.DateRegistrationSent = null;
                _ = await _registrationPersonHelper.UpdateCohortPerson(test);
            } else if (formType.ToLowerInvariant() == "notes") {
                var test = _registrationPersonHelper.GetCohortPerson(id);
                test.InternalComment = Request.Form["internal"].ToString();
                test.ExternalComment = Request.Form["external"].ToString();
                _ = await _registrationPersonHelper.UpdateCohortPerson(test);
            } else if (formType.ToLowerInvariant() == "exemption") {
                var test = _registrationPersonHelper.GetTestPerson(id);
                test.IsProficiencyExemptionApproved = Request.Form["value"].ToString() == "approved";
                test.IsProficiencyExemptionDenied = Request.Form["value"].ToString() == "denied";
                _ = await _registrationPersonHelper.UpdateTestPerson(test);
            } else if (formType.ToLowerInvariant() == "email") {
                _ = await _registrationEmail.SendEmails(id);
            }
            return StatusCode(200);
        }

        public IEnumerable<RegistrationTestPerson> TestPeopleByPerson(int cohortPersonId) => TestPeople.Where(tp => tp.RegistrationCohortPersonId == cohortPersonId);
    }
}