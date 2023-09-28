using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    [IgnoreAntiforgeryToken(Order = 1001)]
    public class RecordingModel : PageModel {
        private readonly AnswerHandler _answerHandler;

        public RecordingModel(AnswerHandler answerHandler) {
            _answerHandler = answerHandler;
        }

        public Answer? Answer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id) {
            Answer = await _answerHandler.GetAnswer(id);
            if (Answer == null || Answer.DurationRecordingInSeconds == 0) {
                return RedirectToPage("./Question", new { id });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            using var ms = new MemoryStream();
            Request.Form.Files.First().CopyTo(ms);
            var fileBytes = ms.ToArray();
            var guid = Guid.Parse(Request.Form["answerguid"]);
            var id = Request.Form["id"];
            _ = await _answerHandler.SetRecording(guid, fileBytes);
            return StatusCode(200);
        }
    }
}