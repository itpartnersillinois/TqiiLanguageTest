using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateTestModel : PageModel {
        private readonly LanguageDbContext _context;

        public CreateTestModel(LanguageDbContext context) {
            _context = context;
        }

        [BindProperty]
        public Test Test { get; set; } = default!;

        public IActionResult OnGet() {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid || _context.Tests == null || Test == null) {
                return Page();
            }

            _context.Tests.Add(Test);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}