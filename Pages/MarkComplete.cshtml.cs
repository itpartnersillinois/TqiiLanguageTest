using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages {

    public class MarkCompleteModel : PageModel {
        private readonly AnswerHandler _answerHandler;

        public MarkCompleteModel(AnswerHandler answerHandler) {
            _answerHandler = answerHandler;
        }

        public async Task<IActionResult> OnGetAsync(Guid id) {
            var answer = await _answerHandler.GetAnswer(id);
            if (answer != null && answer?.Guid != null) {
                _ = await _answerHandler.SetText(answer.Guid, "", true);
            }
            return RedirectToPage("./Question", new { id });
        }
    }
}