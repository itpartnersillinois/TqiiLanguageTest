using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateTestUserModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;
        private readonly TestUserHandler _testUserHandler;

        public CreateTestUserModel(LanguageDbContext context, PermissionsHandler permissions, TestUserHandler testUserHandler) {
            _context = context;
            _permissions = permissions;
            _testUserHandler = testUserHandler;
        }

        [BindProperty]
        public TestUser TestUser { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }

            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "Title");
            TestUser = new TestUser();
            TestUser.OrderBy = 1;
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (_context.TestUsers == null || TestUser == null) {
                return Page();
            }
            _ = await _testUserHandler.AddTestUser(TestUser);

            return RedirectToPage("./Index");
        }
    }
}