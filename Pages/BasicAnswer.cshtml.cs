using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class BasicAnswerModel : PageModel {
        private readonly AnswerHandler _answerHandler;

        public BasicAnswerModel(AnswerHandler answerHandler) {
            _answerHandler = answerHandler;
        }

        public Answer? Answer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id) {
            Answer = await _answerHandler.GetAnswer(id);
            if (Answer != null && Answer.DurationAnswerInSeconds < 60) {
                Answer.DurationAnswerInSeconds = 240;
            }
            if (Answer != null && Answer.InteractiveReadingOptions == "") {
                Answer.InteractiveReadingOptions = "Continue";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var guid = Guid.Parse(Request.Form["answerguid"]);
            var answerText = Request.Form["answertext"];
            var a1 = Request.Form["answertext1"].ToString() ?? "";
            var a2 = Request.Form["answertext2"].ToString() ?? "";
            var a3 = Request.Form["answertext3"].ToString() ?? "";
            var id = Request.Form["id"];
            _ = await _answerHandler.SetBasicQuestion(guid, answerText, a1, a2, a3);
            return RedirectToPage("./Question", new { id });
        }
    }
}