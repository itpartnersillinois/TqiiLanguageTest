using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateQuestionModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public CreateQuestionModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public Question Question { get; set; } = default!;

        public IActionResult OnGet(int id, int questionid) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            if (questionid == 0) {
                Question = new Question { TestId = id };
            } else {
                Question = _context.Questions?.Find(questionid) ?? new Question();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (_context.Questions == null || Question == null) {
                return Page();
            }
            Question.InteractiveReadingAnswer = Question.InteractiveReadingAnswer ?? string.Empty;
            Question.InteractiveReadingOptions = Question.InteractiveReadingOptions ?? string.Empty;
            Question.QuestionText = Question.QuestionText ?? string.Empty;
            Question.AnswerOptions = Question.AnswerOptions ?? string.Empty;
            Question.RecordingText = Question.RecordingText ?? string.Empty;
            if (Question.Id == 0) {
                _context.Questions.Add(Question);
            } else {
                _context.Questions.Update(Question);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./UploadQuestionRecording", new { id = Question.Id, testid = Question.TestId });
        }
    }
}