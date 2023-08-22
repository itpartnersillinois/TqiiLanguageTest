using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages.Admin {

    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
    public class UploadTestRecordingModel : PageModel {
        private readonly PermissionsHandler _permissions;
        private readonly TestHandler _testHandler;

        public UploadTestRecordingModel(TestHandler testHandler, PermissionsHandler permissions) {
            _testHandler = testHandler;
            _permissions = permissions;
        }

        public int AudioType { get; set; }
        public int TestId { get; set; }

        public IActionResult OnGet(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            TestId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            using var ms = new MemoryStream();
            Request.Form.Files.First().CopyTo(ms);
            var fileBytes = ms.ToArray();
            var testid = int.Parse(Request.Form["id"]);
            var audioType = int.Parse(Request.Form["audioType"]);
            var result = await _testHandler.SaveByteArray(testid, fileBytes, audioType);
            return RedirectToPage("./CreateTest", new { id = testid });
        }
    }
}