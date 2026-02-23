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

        private readonly Dictionary<string, TestType> _testTypeLookup = new() {
            { "english", TestType.ProficiencyExam1 },
            { "foreign", TestType.ProficiencyExam2 },
            { "specialed", TestType.SpecialEducation },
            { "interpreter", TestType.Interpreter }
        };

        public CourseModel(RegistrationTestHelper registrationTestHelper, RegistrationPersonHelper registrationPersonHelper, InstructionHelper instructionHelper) {
            _registrationTestHelper = registrationTestHelper;
            _registrationPersonHelper = registrationPersonHelper;
            _instructionHelper = instructionHelper;
        }

        public RegistrationCohort Cohort { get; set; } = default!;
        public int CohortPersonId { get; set; }
        public string Conclusion { get; set; } = "";
        public string EnglishProficiency { get; set; } = "";
        public IEnumerable<RegistrationTest> EnglishProficiencyTests => Tests.Where(t => t.TypeOfTest == TestType.ProficiencyExam1);
        public string ForeignLanguageProficiency { get; set; } = "";

        public IEnumerable<RegistrationTest> ForeignLanguageProficiencyTests => Tests.Where(t => t.TypeOfTest == TestType.ProficiencyExam2);
        public IEnumerable<string> ForeignLanguages { get; set; } = default!;
        public string Interpreter { get; set; } = "";
        public IEnumerable<RegistrationTest> InterpreterCourses => Tests.Where(t => t.TypeOfTest == TestType.Interpreter);
        public bool IsAlreadyRegistered { get; set; } = false;

        [BindProperty]
        public RegistrationPerson RegistrationPerson { get; set; } = default!;

        public string SpecialEducation { get; set; } = "";
        public IEnumerable<RegistrationTest> SpecialEducationCourses => Tests.Where(t => t.TypeOfTest == TestType.SpecialEducation);
        public List<RegistrationTest> Tests { get; set; } = default!;

        public async Task<IActionResult> OnGet() {
            var cohortId = string.IsNullOrWhiteSpace(Request.Query["cohortId"]) ? 0 : int.Parse(Request.Query["cohortId"]);
            RegistrationPerson = _registrationPersonHelper.GetPerson(User.Identity?.Name ?? "");
            Cohort = _registrationTestHelper.GetCohort(cohortId);
            Tests = _registrationTestHelper.GetTests(cohortId);

            EnglishProficiency = _instructionHelper.GetInstructionString(InstructionType.LangaugeProficiency1);
            ForeignLanguageProficiency = _instructionHelper.GetInstructionString(InstructionType.LangaugeProficiency2);
            SpecialEducation = _instructionHelper.GetInstructionString(InstructionType.SpedProficiency);
            Interpreter = _instructionHelper.GetInstructionString(InstructionType.InterpreterProficiency);
            Conclusion = _instructionHelper.GetInstructionString(InstructionType.Conclusion);
            var languageTests = Tests.Select(t => t.Language.ToLowerInvariant()).Distinct();
            ForeignLanguages = _registrationTestHelper.GetLanguages().Where(l => l.ToLowerInvariant() != "english" && !l.ToLowerInvariant().Contains("pilot") && !languageTests.Contains(l.ToLowerInvariant()));

            var assignedCohort = _registrationPersonHelper.IsPersonAssignedToCohortGetId(RegistrationPerson.Id);
            if (assignedCohort == null || assignedCohort == 0) {
                CohortPersonId = await _registrationPersonHelper.AssignPersonToCohort(Cohort.Id, RegistrationPerson.Id);
            } else {
                CohortPersonId = assignedCohort.Value;
                IsAlreadyRegistered = true;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var testId = string.IsNullOrWhiteSpace(Request.Form["testid"]) ? 0 : int.Parse(Request.Form["testid"]);
            var cohortPersonId = string.IsNullOrWhiteSpace(Request.Form["cohortpersonid"]) ? 0 : int.Parse(Request.Form["cohortpersonid"]);
            var isComplete = string.IsNullOrWhiteSpace(Request.Form["complete"]) ? false : bool.Parse(Request.Form["complete"]);
            if (isComplete) {
                _ = await _registrationPersonHelper.MarkPersonCohortAsComplete(cohortPersonId);
            } else {
                var isExempt = string.IsNullOrWhiteSpace(Request.Form["exempt"]) ? true : bool.Parse(Request.Form["exempt"]);
                var language = string.IsNullOrWhiteSpace(Request.Form["language"]) ? "" : Request.Form["language"].ToString();
                var id = await _registrationPersonHelper.AssignPersonToTest(testId, cohortPersonId, isExempt, language);

                if (Request.Form.Files.Count > 0) {
                    using var ms = new MemoryStream();
                    Request.Form.Files.First().CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var filename = Request.Form["filename"];
                    var testTypeString = Request.Form["testtype"];
                    var testType = _testTypeLookup.ContainsKey(testTypeString) ? _testTypeLookup[testTypeString] : TestType.Other;
                    _ = await _registrationPersonHelper.AssignDocumentToTest(id, fileBytes, filename, testType);
                }
            }
            return StatusCode(200);
        }
    }
}