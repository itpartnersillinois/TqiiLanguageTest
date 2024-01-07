using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListTestUserModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ListTestUserModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<TestUser> TestUser { get; set; } = default!;

        public string TestUserIds => string.Join('-', TestUser.Select(tu => tu.Id));

        public async Task OnGetAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var take = string.IsNullOrWhiteSpace(Request.Query["take"]) ? 50 : int.Parse(Request.Query["take"]);
            var skip = string.IsNullOrWhiteSpace(Request.Query["skip"]) ? 0 : take * int.Parse(Request.Query["skip"]);
            var search = Request.Query["search"].ToString();

            if (_context.TestUsers != null) {
                if (string.IsNullOrEmpty(search)) {
                    TestUser = await _context.TestUsers.OrderByDescending(tu => tu.DateTimeStart).Skip(skip).Take(take)
                    .Include(t => t.Test).Select(tu => new TestUser { Id = tu.Id, UserIdentification = tu.UserIdentification, Email = tu.Email, CurrentQuestionOrder = tu.CurrentQuestionOrder, DateTimeStart = tu.DateTimeStart, DateTimeEnd = tu.DateTimeEnd, NumberReviewers = tu.NumberReviewers, NumberReviewerScores = tu.NumberReviewerScores, Score = tu.Score, Test = new Test { Title = tu.Test.Title, Id = tu.Test.Id } }).ToListAsync();
                } else {
                    TestUser = await _context.TestUsers.Where(tu => tu.Email.Contains(search) || tu.UserIdentification.Contains(search)).OrderByDescending(tu => tu.DateTimeStart).Skip(skip).Take(take)
                    .Include(t => t.Test).Select(tu => new TestUser { Id = tu.Id, UserIdentification = tu.UserIdentification, Email = tu.Email, CurrentQuestionOrder = tu.CurrentQuestionOrder, DateTimeStart = tu.DateTimeStart, DateTimeEnd = tu.DateTimeEnd, NumberReviewers = tu.NumberReviewers, NumberReviewerScores = tu.NumberReviewerScores, Score = tu.Score, Test = new Test { Title = tu.Test.Title, Id = tu.Test.Id } }).ToListAsync();
                }
            }
        }
    }
}