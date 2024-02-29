using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateTestUserModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;
        private readonly TestUserHandler _testUserHandler;

        public CreateTestUserModel(LanguageDbContext context, PermissionsHandler permissions, TestUserHandler testUserHandler) {
            _context = context;
            _permissions = permissions;
            _testUserHandler = testUserHandler;
        }

        [BindProperty]
        public int? TestId2 { get; set; } = default!;

        [BindProperty]
        public int? TestId3 { get; set; } = default!;

        [BindProperty]
        public int? TestId4 { get; set; } = default!;

        [BindProperty]
        public TestUser TestUser { get; set; } = default!;

        public IActionResult OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var dictionary = _context.Tests?.Select(t => new { t.Id, t.Title, t.Language }).OrderBy(t => t.Language).ThenBy(t => t.Title).ToDictionary(t => t.Id, t => t.Title + " (" + t.Language + ")");
            var dictionaryNew = dictionary.ToDictionary(t => t.Key, t => t.Value);
            dictionaryNew.Add(0, "");
            ViewData["TestId"] = new SelectList(dictionary, "Key", "Value");
            ViewData["TestIdOptional"] = new SelectList(dictionaryNew, "Key", "Value", 0);
            TestUser = new TestUser();
            TestUser.OrderBy = 1;
            ViewData["Languages"] = new SelectList(_context.LanguageOptions?.Select(l => l.Language).ToList());
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (_context.TestUsers == null || TestUser == null) {
                return Page();
            }
            if (TestUser.Email.Contains(',')) {
                var emailArray = TestUser.Email.Split(',');
                foreach (var email in emailArray) {
                    _ = await _testUserHandler.AddTestUser(new TestUser {
                        Email = email.Trim(),
                        OrderBy = TestUser.OrderBy,
                        DateTimeScheduled = TestUser.DateTimeScheduled,
                        TestId = TestUser.TestId,
                        Language = TestUser.Language
                    });
                }
            } else {
                _ = await _testUserHandler.AddTestUser(TestUser);
            }
            _ = await RunTest(TestId2, 1);
            _ = await RunTest(TestId3, 2);
            _ = await RunTest(TestId4, 3);
            return RedirectToPage("./Index");
        }

        public async Task<bool> RunTest(int? testId, int orderByIncrease) {
            if (testId.HasValue && testId != 0) {
                var emailArray = TestUser.Email.Split(',');
                foreach (var email in emailArray) {
                    _ = await _testUserHandler.AddTestUser(new TestUser {
                        Email = email.Trim(),
                        DateTimeScheduled = TestUser.DateTimeScheduled,
                        OrderBy = TestUser.OrderBy + orderByIncrease,
                        TestId = testId.Value,
                        Language = TestUser.Language
                    });
                }
                return true;
            }
            return false;
        }
    }
}