using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.Registration {

    [IgnoreAntiforgeryToken(Order = 1001)]
    public class CourseModel : PageModel {
        private readonly InstructionHelper _instructionHelper;
        private readonly RegistrationPersonHelper _registrationPersonHelper;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public CourseModel(RegistrationTestHelper registrationTestHelper, RegistrationPersonHelper registrationPersonHelper, InstructionHelper instructionHelper) {
            _registrationTestHelper = registrationTestHelper;
            _registrationPersonHelper = registrationPersonHelper;
            _instructionHelper = instructionHelper;
        }

        public RegistrationCohort Cohort { get; set; } = default!;
        public int CohortPersonId { get; set; }
        public string EnglishProficiency { get; set; } = "";
        public IEnumerable<RegistrationTest> EnglishProficiencyTests => Tests.Where(t => t.TypeOfTest == TestType.ProficiencyExam1);

        [BindProperty]
        public RegistrationPerson RegistrationPerson { get; set; } = default!;

        public List<RegistrationTest> Tests { get; set; } = default!;

        public async Task<IActionResult> OnGet() {
            var cohortId = string.IsNullOrWhiteSpace(Request.Query["cohortId"]) ? 0 : int.Parse(Request.Query["cohortId"]);
            RegistrationPerson = _registrationPersonHelper.GetPerson(User.Identity?.Name ?? "");
            Cohort = _registrationTestHelper.GetCohort(cohortId);
            Tests = _registrationTestHelper.GetTests(cohortId);
            EnglishProficiency = _instructionHelper.GetInstructionString(InstructionType.LangaugeProficiency1);
            CohortPersonId = await _registrationPersonHelper.AssignPersonToCohort(Cohort.Id, RegistrationPerson.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var testId = string.IsNullOrWhiteSpace(Request.Form["testid"]) ? 0 : int.Parse(Request.Form["testid"]);
            var cohortPersonId = string.IsNullOrWhiteSpace(Request.Form["cohortpersonid"]) ? 0 : int.Parse(Request.Form["cohortpersonid"]);
            var id = await _registrationPersonHelper.AssignPersonToTest(testId, cohortPersonId);

            if (Request.Form.Files.Count > 0) {
                using var ms = new MemoryStream();
                Request.Form.Files.First().CopyTo(ms);
                var fileBytes = ms.ToArray();
                var filename = Request.Form["filename"];
                _ = await _registrationPersonHelper.AssignDocumentToTest(id, fileBytes, filename);
            }
            return StatusCode(200);
        }
    }
}