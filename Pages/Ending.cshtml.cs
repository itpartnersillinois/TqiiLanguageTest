using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages {

    public class EndingModel : PageModel {
        private readonly PracticeTestHandler _practiceTestHandler;
        private readonly TestHandler _testHandler;
        private readonly TestUserHandler _testUserHandler;

        public EndingModel(TestHandler testHandler, TestUserHandler testUserHandler, PracticeTestHandler practiceTestHandler) {
            _testHandler = testHandler;
            _testUserHandler = testUserHandler;
            _practiceTestHandler = practiceTestHandler;
        }

        public Guid Guid { get; set; }

        public Guid? NewTestGuid { get; set; }
        public Test? Test { get; set; } = default!;

        public TestUser? TestUser { get; set; } = default!;

        public IActionResult OnGet(Guid id) {
            Guid = id;
            Test = _testHandler.GetTest(id);
            TestUser = _testUserHandler.GetTestUser(id);
            NewTestGuid = Test != null && Test.IsPractice
                ? _practiceTestHandler.GetTestUserGuid(User.Identity?.Name ?? "", false)
                : _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "");
            return Page();
        }
    }
}