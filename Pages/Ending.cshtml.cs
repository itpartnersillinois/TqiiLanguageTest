using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class EndingModel : PageModel {
        private readonly TestHandler _testHandler;
        private readonly TestUserHandler _testUserHandler;

        public EndingModel(TestHandler testHandler, TestUserHandler testUserHandler) {
            _testHandler = testHandler;
            _testUserHandler = testUserHandler;
        }

        public Guid Guid { get; set; }
        public Test? Test { get; set; } = default!;

        public TestUser? TestUser { get; set; } = default!;

        public IActionResult OnGet(Guid id) {
            Guid = id;
            Test = _testHandler.GetTest(id);
            TestUser = _testUserHandler.GetTestUser(id);
            return Page();
        }
    }
}