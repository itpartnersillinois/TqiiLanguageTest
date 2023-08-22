using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class StartingModel : PageModel {
        private readonly TestHandler _testHandler;

        public StartingModel(TestHandler testHandler) {
            _testHandler = testHandler;
        }

        public Guid Guid { get; set; }
        public Test? Test { get; set; } = default!;

        public IActionResult OnGet(Guid id) {
            Guid = id;
            Test = _testHandler.GetTest(id);
            return Page();
        }
    }
}