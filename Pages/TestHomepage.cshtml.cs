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

        public List<TestObject> TestObjects { get; set; } = new List<TestObject>();

        public void OnGet() {
            // TODO Asked to temporarily remove the "must take practice test" -- need to re-add this later
            // Guid = _testUserHandler.DidUserCompleteAnyTests(User.Identity?.Name ?? "")
            //     ? _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "")
            //     : null;

            TestObjects = _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "").Select(i =>
            new TestObject {
                Guid = i.Item1,
                TimeActive = i.Item2,
                TimeExpired = i.Item3,
                Language = i.Item4,
            }).ToList();

            foreach (var testObject in TestObjects) {
                testObject.PracticeGuid = _practiceTestHandler.GetTestUserGuid(User.Identity?.Name ?? "", true, testObject.Language);
            }
        }
    }

    public class TestObject {
        public Guid? Guid { get; set; }
        public string Language { get; set; } = "";
        public Guid? PracticeGuid { get; set; }
        public DateTime? TimeActive { get; set; }
        public DateTime? TimeExpired { get; set; }
    }
}