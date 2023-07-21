using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateQuestionModel : PageModel {
        private readonly LanguageDbContext _context;

        public CreateQuestionModel(LanguageDbContext context) {
            _context = context;
        }

        [BindProperty]
        public Question Question { get; set; } = default!;

        public IActionResult OnGet() {
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid || _context.Questions == null || Question == null) {
                return Page();
            }

            _context.Questions.Add(Question);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}