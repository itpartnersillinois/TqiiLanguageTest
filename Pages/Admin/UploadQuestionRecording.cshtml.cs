using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages.Admin {

    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
    public class UploadQuestionRecordingModel : PageModel {
        private readonly PermissionsHandler _permissions;
        private readonly QuestionHandler _questionHandler;

        public UploadQuestionRecordingModel(QuestionHandler questionHandler, PermissionsHandler permissions) {
            _questionHandler = questionHandler;
            _permissions = permissions;
        }

        public int Id { get; set; }
        public int ImageType { get; set; }
        public int TestId { get; set; }

        public IActionResult OnGet(int id, int testid) {
            if (!_permissions.IsItemWriter(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            Id = id;
            TestId = testid;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            using var ms = new MemoryStream();
            var id = int.Parse(Request.Form.First().Value);
            var testid = Request.Form["testid"];
            var imagetype = int.Parse(Request.Form["imageType"]);
            if (Request.Form.Files.Count == 0) {
                _ = await _questionHandler.SaveByteArray(id, Array.Empty<byte>(), (ImageTypeEnum)imagetype);
                return RedirectToPage("./CreateTest", new { id = testid });
            }
            Request.Form.Files.First().CopyTo(ms);
            var fileBytes = ms.ToArray();
            var result = await _questionHandler.SaveByteArray(id, fileBytes, (ImageTypeEnum)imagetype);
            return RedirectToPage("./CreateTest", new { id = testid });
        }
    }
}