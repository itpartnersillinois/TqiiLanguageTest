using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.Registration {

    public class IndexModel : PageModel {
        private readonly InstructionHelper _instructionHelper;
        private readonly RegistrationPersonHelper _registrationPersonHelper;
        private readonly RegistrationTestHelper _registrationTestHelper;

        public IndexModel(RegistrationTestHelper registrationTestHelper, RegistrationPersonHelper registrationPersonHelper, InstructionHelper instructionHelper) {
            _registrationTestHelper = registrationTestHelper;
            _registrationPersonHelper = registrationPersonHelper;
            _instructionHelper = instructionHelper;
        }

        public List<RegistrationCohort> Cohorts { get; set; } = default!;
        public string Introduction { get; set; } = "";

        [BindProperty]
        public RegistrationPerson RegistrationPerson { get; set; } = default!;

        public IActionResult OnGet() {
            RegistrationPerson = _registrationPersonHelper.GetPerson(User.Identity?.Name ?? "");
            Cohorts = _registrationTestHelper.GetCohorts();
            Introduction = _instructionHelper.GetInstructionString(InstructionType.Introduction);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var returnValue = await _registrationPersonHelper.SavePerson(RegistrationPerson, User.Identity?.Name ?? "");
            return this.RedirectToPage();
        }
    }
}