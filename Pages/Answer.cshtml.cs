using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class AnswerModel : PageModel {
        private readonly AnswerHandler _answerHandler;

        public AnswerModel(AnswerHandler answerHandler) {
            _answerHandler = answerHandler;
        }

        public Answer? Answer { get; set; } = default!;

        public List<string> ButtonAnswers { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id) {
            Answer = await _answerHandler.GetAnswer(id);
            if (Answer == null) {
                return RedirectToPage("./Recording", new { id });
            }
            ButtonAnswers = Answer.AnswerOptions.Split("|").ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var guid = Guid.Parse(Request.Form["answerguid"]);
            var answerText = Request.Form["answertext"];
            var id = Request.Form["id"];
            _ = await _answerHandler.SetText(guid, answerText);
            return RedirectToPage("./Recording", new { id });
        }
    }
}