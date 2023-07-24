using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages.Admin {

    public class UploadQuestionRecordingModel : PageModel {
        private readonly PermissionsHandler _permissions;
        private readonly QuestionHandler _questionHandler;

        public UploadQuestionRecordingModel(QuestionHandler questionHandler, PermissionsHandler permissions) {
            _questionHandler = questionHandler;
            _permissions = permissions;
        }

        public int Id { get; set; }

        public int TestId { get; set; }

        public IActionResult OnGet(int id, int testid) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            Id = id;
            TestId = testid;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            using var ms = new MemoryStream();
            Request.Form.Files.First().CopyTo(ms);
            var fileBytes = ms.ToArray();
            var id = int.Parse(Request.Form.First().Value);
            var testid = Request.Form["testid"];
            var result = await _questionHandler.SaveRecording(id, fileBytes);
            return RedirectToPage("./CreateQuestion", new { id = testid });
        }
    }
}