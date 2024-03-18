using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateTestUserLanguageModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;
        private readonly TestUserHandler _testUserHandler;

        public CreateTestUserLanguageModel(LanguageDbContext context, PermissionsHandler permissions, TestUserHandler testUserHandler) {
            _context = context;
            _permissions = permissions;
            _testUserHandler = testUserHandler;
        }

        [BindProperty]
        public string Language { get; set; } = "";

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            ViewData["Languages"] = new SelectList(_context.LanguageOptions?.Select(l => l.Language).ToList());
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            return RedirectToPage("./CreateTestUser", new { language = Language });
        }
    }
}