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

        public DateTime? TimeActive { get; set; }

        public void OnGet() {
            // TODO Asked to temporarily remove the "must take practice test" -- need to re-add this later
            // Guid = _testUserHandler.DidUserCompleteAnyTests(User.Identity?.Name ?? "")
            //     ? _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "")
            //     : null;

            var test = _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "");
            Guid = test.Item1;
            TimeActive = test.Item2;
            PracticeGuid = _practiceTestHandler.GetTestUserGuid(User.Identity?.Name ?? "", true);
        }
    }
}