using System.Text;
using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("registrationdocument")]
    public class RegistrationDocumentController : Controller {
        private readonly RegistrationDbContext _context;
        private readonly PermissionsHandler _permissions;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public RegistrationDocumentController(PermissionsHandler permissions, RegistrationDbContext context, RegistrationTestHelper registrationTestHelper) {
            _permissions = permissions;
            _context = context;
            _registrationTestHelper = registrationTestHelper;
        }

        [HttpGet("cohort/{id}")]
        public IActionResult CohortList(int id) {
            if (!_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "") && !_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var cohort = _registrationTestHelper.GetCohort(id);
            var cohortPeople = _registrationTestHelper.GetCohortPeople(id);
            var testPeople = _registrationTestHelper.GetTestPeople(id);

            var sb = new StringBuilder();

            _ = sb.AppendLine($"Cohort List for {cohort.TestName} ({cohort.DateString})");
            foreach (var cohortPerson in cohortPeople) {
                _ = sb.Append($"{cohortPerson.RegistrationPerson?.FirstName} {@cohortPerson.RegistrationPerson?.LastName}\t");
                _ = sb.Append($"{cohortPerson.RegistrationPerson?.Email}\t");
                foreach (var test in testPeople.Where(tp => tp.RegistrationCohortPersonId == cohortPerson.Id).OrderBy(c => c.RegistrationTest?.TypeOfTest)) {
                    _ = sb.Append($"{test.RegistrationTest?.TestName} ({test.RegistrationTest?.TypeOfTest})\t");
                    _ = sb.Append($"{(test.IsProficiencyExemption ? "Requested Exemption" : "")}\t\t");
                }
                _ = sb.AppendLine();
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", $"cohort-list-{id}.txt");
        }

        [HttpGet("{id}")]
        public void Index(int id) {
            if (!_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "") && !_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var storage = _context?.Documents?.SingleOrDefault(a => a.Id == id);
            if (storage == null || storage.Document == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            var stream = new MemoryStream(storage.Document);
            stream.CopyToAsync(Response.Body);
        }
    }
}