using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages {

    public class TestHomepageModel : PageModel {
        private readonly PracticeTestHandler _practiceTestHandler;
        private readonly TestUserHandler _testUserHandler;

        public TestHomepageModel(TestUserHandler testUserHandler, PracticeTestHandler practiceTestHandler) {
            _testUserHandler = testUserHandler;
            _practiceTestHandler = practiceTestHandler;
        }

        public Guid? Guid { get; set; }
        public Guid? PracticeGuid { get; set; }

        public void OnGet() {
            Guid = _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "");
            PracticeGuid = _practiceTestHandler.GetTestUserGuid(User.Identity?.Name ?? "", true);
        }
    }
}