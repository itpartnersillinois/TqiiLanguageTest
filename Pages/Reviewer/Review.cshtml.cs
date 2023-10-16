using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Reviewer {

    public class ReviewModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ReviewModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public Answer Answer { get; set; } = default!; // this does not have all the properties, so do not use this as a true bindable object

        public int Rating { get; set; }

        public void OnGet(int id) {
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.Answers != null) {
                Answer = _context.Answers.Include(a => a.Question).First(a => a.Id == id);
            }

            if (int.TryParse(Answer.RubricInformation, out int x)) {
                Rating = x;
            } else {
                Rating = 9;
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (_context != null && _context.Answers != null) {
                var answerUpdate = _context.Answers.Include(a => a.Question).First(a => a.Id == Answer.Id);

                answerUpdate.ReviewerNotes = Answer.ReviewerNotes ?? string.Empty;
                answerUpdate.RubricInformation = Request.Form["level"].ToString();
                _context.Answers.Update(answerUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./TestUser", new { id = answerUpdate.TestUserId });
            }
            return RedirectToPage("./Index");
        }
    }
}