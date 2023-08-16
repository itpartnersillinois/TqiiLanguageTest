using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class QuestionModel : PageModel {
        private readonly QuestionHandler _questionHandler;

        public QuestionModel(QuestionHandler questionHandler) {
            _questionHandler = questionHandler;
        }

        public Question? Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id) {
            Question = await _questionHandler.GetQuestion(id);
            if (Question == null) {
                return RedirectToPage("./Ending");
            }
            if (Question.QuestionType == QuestionEnum.InteractiveReading) {
                return RedirectToPage("./ReadingAnswer", new { id });
            }
            return Page();
        }
    }
}