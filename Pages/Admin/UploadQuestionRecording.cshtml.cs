using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages.Admin {

    public class UploadQuestionRecordingModel : PageModel {
        private readonly QuestionHandler _questionHandler;

        public UploadQuestionRecordingModel(QuestionHandler questionHandler) {
            _questionHandler = questionHandler;
        }

        [BindProperty]
        public int Id { get; set; }

        public void OnGet(int id) {
            Id = id;
        }

        public async Task<IActionResult> OnPostAsync() {
            using var ms = new MemoryStream();
            Request.Form.Files.First().CopyTo(ms);
            var fileBytes = ms.ToArray();
            var id = int.Parse(Request.Form.First().Value);
            var result = await _questionHandler.SaveRecording(id, fileBytes);
            return new EmptyResult();
        }
    }
}