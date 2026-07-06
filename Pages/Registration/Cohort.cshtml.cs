using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.Registration {

    public class CohortModel : PageModel {
        private readonly InstructionHelper _instructionHelper;
        private readonly RegistrationPersonHelper _registrationPersonHelper;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public CohortModel(RegistrationTestHelper registrationTestHelper, RegistrationPersonHelper registrationPersonHelper, InstructionHelper instructionHelper) {
            _registrationTestHelper = registrationTestHelper;
            _registrationPersonHelper = registrationPersonHelper;
            _instructionHelper = instructionHelper;
        }

        public RegistrationCohort? AssignedCohort { get; set; }
        public string CohortIntroduction { get; set; } = "";
        public List<RegistrationCohort> Cohorts { get; set; } = default!;
        public string Iein { get; set; } = "";
        public string Introduction { get; set; } = "";

        [BindProperty]
        public RegistrationPerson RegistrationPerson { get; set; } = default!;

        public IActionResult OnGet() {
            var name = User.Identity?.Name ?? "";
            RegistrationPerson = _registrationPersonHelper.GetPerson(name);
            Cohorts = _registrationTestHelper.GetCohorts(RegistrationPerson.Id);
            Introduction = _instructionHelper.GetInstructionString(InstructionType.Introduction);
            CohortIntroduction = _instructionHelper.GetInstructionString(InstructionType.CohortIntroduction);
            Iein = _instructionHelper.GetInstructionString(InstructionType.Iein);
            AssignedCohort = _registrationPersonHelper.IsPersonAssignedToCohort(RegistrationPerson.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var cohortId = Request.Form["cohortid"];
            if (string.IsNullOrWhiteSpace(cohortId)) {
                return RedirectToPage("/Registration/Cohort");
            }
            return RedirectToPage("/Registration/Course", new { cohortid = cohortId });
        }
    }
}