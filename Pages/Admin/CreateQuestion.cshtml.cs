using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            var rubrics = _context.RaterScales?.Select(r => r.RaterScaleName).OrderBy(s => s).Distinct().ToList();
            rubrics?.Insert(0, "");
            ViewData["Rubrics"] = new SelectList(rubrics);

            if (questionid == 0) {
                var language = _context.Tests?.Find(id)?.Language ?? "";
                Question = new Question { TestId = id, Language = language };
            } else {
                Question = _context.Questions?.Find(questionid) ?? new Question();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (_context.Questions == null || Question == null) {
                return Page();
            }
            var testid = Question.TestId;
            Question.Language = Question.Language ?? string.Empty;
            Question.IntroductionText = Question.IntroductionText ?? string.Empty;
            Question.InteractiveReadingAnswer = Question.InteractiveReadingAnswer ?? string.Empty;
            Question.InteractiveReadingOptions = Question.InteractiveReadingOptions ?? string.Empty;
            Question.InteractiveReadingOptionsAnswerKey = Question.InteractiveReadingOptionsAnswerKey ?? string.Empty;
            Question.InteractiveReadingOptionsDropDown = Question.InteractiveReadingOptionsDropDown ?? string.Empty;
            Question.QuestionText = Question.QuestionText ?? string.Empty;
            Question.AnswerOptions = Question.AnswerOptions ?? string.Empty;
            Question.RecordingText = Question.RecordingText ?? string.Empty;
            Question.BasicAnswers1 = Question.BasicAnswers1 ?? string.Empty;
            Question.BasicAnswers2 = Question.BasicAnswers2 ?? string.Empty;
            Question.BasicAnswers3 = Question.BasicAnswers3 ?? string.Empty;
            Question.BasicAnswerKey1 = Question.BasicAnswerKey1 ?? string.Empty;
            Question.BasicAnswerKey2 = Question.BasicAnswerKey2 ?? string.Empty;
            Question.BasicAnswerKey3 = Question.BasicAnswerKey3 ?? string.Empty;
            Question.BasicQuestion1 = Question.BasicQuestion1 ?? string.Empty;
            Question.BasicQuestion2 = Question.BasicQuestion2 ?? string.Empty;
            Question.BasicQuestion3 = Question.BasicQuestion3 ?? string.Empty;
            Question.SentenceRepetionText = Question.SentenceRepetionText ?? string.Empty;
            if (string.IsNullOrWhiteSpace(Question.Title)) {
                Question.OrderBy = -1;
                Question.Title = "Deleted question on " + DateTime.Now.ToShortDateString();
            }
            if (Question.Id == 0) {
                _context.Questions.Add(Question);
            } else {
                var baseQuestion = _context.Questions.AsNoTracking().First(q => q.Id == Question.Id);
                Question.InteractiveReadingImage = baseQuestion.InteractiveReadingImage;
                Question.IntroductionImage = baseQuestion.IntroductionImage;
                Question.QuestionImage = baseQuestion.QuestionImage;
                Question.RecordingImage = baseQuestion.RecordingImage;
                Question.Recording = baseQuestion.Recording;
                _context.Questions.Update(Question);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./CreateTest", new { id = testid });
        }
    }
}