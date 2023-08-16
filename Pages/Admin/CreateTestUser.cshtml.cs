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

        public CreateTestUserModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public TestUser TestUser { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }

            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "Title");
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid || _context.TestUsers == null || TestUser == null) {
                return Page();
            }
            var test = _context.Tests?.Find(TestUser.TestId) ?? new Test();

            TestUser.TotalQuestions = test.NumberQuestions;

            _context.TestUsers.Add(TestUser);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}