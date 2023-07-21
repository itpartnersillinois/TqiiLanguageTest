using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class QuestionTestModel : PageModel {
        private readonly LanguageDbContext _context;

        public QuestionTestModel(LanguageDbContext context) {
            _context = context;
        }

        public Question Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null || _context.Questions == null) {
                return NotFound();
            }

            var question = await _context.Questions.FirstOrDefaultAsync(m => m.Id == id);
            if (question == null) {
                return NotFound();
            } else {
                Question = question;
            }
            return Page();
        }
    }
}