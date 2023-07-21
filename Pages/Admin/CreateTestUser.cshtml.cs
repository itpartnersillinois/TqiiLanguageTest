using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateTestUserModel : PageModel {
        private readonly LanguageDbContext _context;

        public CreateTestUserModel(LanguageDbContext context) {
            _context = context;
        }

        [BindProperty]
        public TestUser TestUser { get; set; } = default!;

        public IActionResult OnGet() {
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid || _context.TestUsers == null || TestUser == null) {
                return Page();
            }

            _context.TestUsers.Add(TestUser);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}