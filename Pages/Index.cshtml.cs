using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;

namespace TqiiLanguageTest.Pages {

    public class IndexModel : PageModel {
        private readonly TestUserHandler _testUserHandler;

        public IndexModel(TestUserHandler testUserHandler) {
            _testUserHandler = testUserHandler;
        }

        public Guid? Guid { get; set; }

        public void OnGet() {
            Guid = _testUserHandler.GetTestUserGuid(User.Identity?.Name ?? "");
        }
    }
}