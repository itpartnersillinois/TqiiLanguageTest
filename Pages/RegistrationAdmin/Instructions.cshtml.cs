using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Pages.RegistrationAdmin {

    public class InstructionsModel : PageModel {
        private readonly InstructionHelper _instructionHelper;
        private readonly PermissionsHandler _permissions;

        public InstructionsModel(PermissionsHandler permissions, InstructionHelper instructionHelper) {
            _permissions = permissions;
            _instructionHelper = instructionHelper;
        }

        [BindProperty]
        public RegistrationInstruction RegistrationInstruction { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var id = string.IsNullOrWhiteSpace(Request.Query["id"]) ? 0 : int.Parse(Request.Query["id"]);
            RegistrationInstruction = _instructionHelper.GetInstruction((InstructionType) id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "") && !_permissions.IsRegistrationReviewer(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            _ = await _instructionHelper.Save(RegistrationInstruction);
            return Page();
        }
    }
}