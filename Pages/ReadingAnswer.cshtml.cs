using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class ReadingAnswerModel : PageModel {
        private readonly AnswerHandler _answerHandler;

        public ReadingAnswerModel(AnswerHandler answerHandler) {
            _answerHandler = answerHandler;
        }

        public Answer? Answer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id) {
            Answer = await _answerHandler.GetAnswer(id);
            if (Answer != null && Answer.DurationAnswerInSeconds < 60) {
                Answer.DurationAnswerInSeconds = 240;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var guid = Guid.Parse(Request.Form["answerguid"]);
            var answerText = Request.Form["answertext"];
            var id = Request.Form["id"];
            _ = await _answerHandler.SetText(guid, answerText, true);
            return RedirectToPage("./Question", new { id });
        }
    }
}