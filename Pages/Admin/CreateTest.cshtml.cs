using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateTestModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public CreateTestModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<Question> Questions { get; set; } = default!;

        [BindProperty]
        public Test Test { get; set; } = default!;

        public IActionResult OnGet(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            if (id != 0) {
                Test = _context.Tests?.Find(id) ?? new Test();
                Questions = _context.Questions?.Where(q => q.TestId == id).ToList() ?? new List<Question>();
            } else {
                Questions = new List<Question>();
            }
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (_context.Tests == null || Test == null) {
                return Page();
            }

            if (Test.Id == 0) {
                _context.Tests.Add(Test);
            } else {
                _context.Tests.Update(Test);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./ListTest");
        }
    }
}