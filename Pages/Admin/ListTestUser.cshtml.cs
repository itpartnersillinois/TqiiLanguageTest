using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListTestUserModel : PageModel {
        private readonly Dictionary<string, DateTime> _cohorts = new() { { "cohort1", DateTime.Parse("8/12/2024 11:00:00 AM") } };
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ListTestUserModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public IList<TestUser> TestUser { get; set; } = default!;

        public string TestUserIds => string.Join('-', TestUser.Select(tu => tu.Id));

        public async Task OnGetAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            _ = _context.Database.ExecuteSqlRaw("exec dbo.ResetReviewerStats");
            var take = string.IsNullOrWhiteSpace(Request.Query["take"]) ? 50 : int.Parse(Request.Query["take"]);
            var skip = string.IsNullOrWhiteSpace(Request.Query["skip"]) ? 0 : take * int.Parse(Request.Query["skip"]);
            var search = Request.Query["search"].ToString();
            var testsearch = Request.Query["testsearch"].ToString();
            var filter = Request.Query["filter"].ToString() ?? "";
            var sort = Request.Query["sort"].ToString() ?? "";
            var daterange = Request.Query["daterange"].ToString() ?? "";
            var date = _cohorts.ContainsKey(daterange) ? _cohorts[daterange] : DateTime.MinValue;

            if (_context.TestUsers != null) {
                Expression<Func<TestUser, bool>> whereLambda = filter == "not-started" ? tu => tu.DateTimeStart == null :
                    filter == "in-process" ? tu => tu.DateTimeStart != null && tu.DateTimeEnd == null :
                    filter == "test-completed" ? tu => tu.DateTimeEnd != null :
                    filter == "restarts" ? tu => tu.NumberTimesRefreshed > 0 :
                    filter == "need-reviewers" ? tu => tu.DateTimeEnd != null && tu.NumberReviewers == 0 :
                    filter == "has-reviewers" ? tu => tu.NumberReviewers > 0 && tu.NumberReviewerScores != tu.NumberReviewers && tu.Score == 0 :
                    filter == "reviews-completed" ? tu => tu.NumberReviewers > 0 && tu.NumberReviewerScores == tu.NumberReviewers && tu.Score == 0 :
                    filter == "scored" ? tu => tu.NumberReviewers > 0 && tu.NumberReviewerScores == tu.NumberReviewers && tu.Score == 0 : tu => true;

                var recordCount = _context.TestUsers.Include(t => t.Test).Where(whereLambda)
                        .Where(tu => tu.DateTimeStart > date || tu.DateTimeScheduled > date || (tu.DateTimeStart == null && tu.DateTimeScheduled == null))
                        .Where(tu => search == "" || (tu.Email.Contains(search) || tu.UserIdentification.Contains(search)))
                        .Where(tu => testsearch == "" || tu.Test.Title.Contains(testsearch)).Count();

                PageCount = (int) Math.Ceiling(recordCount / (double) take);
                CurrentPage = (skip / take) == 0 ? 1 : (skip / take);

                if (sort == "test") {
                    TestUser = await _context.TestUsers.Include(t => t.Test).Where(whereLambda)
                        .Where(tu => tu.DateTimeStart > date || tu.DateTimeScheduled > date || (tu.DateTimeStart == null && tu.DateTimeScheduled == null))
                        .Where(tu => search == "" || (tu.Email.Contains(search) || tu.UserIdentification.Contains(search)))
                        .Where(tu => testsearch == "" || tu.Test.Title.Contains(testsearch))
                        .OrderBy(tu => tu.Test.Title).ThenByDescending(tu => tu.DateTimeStart).Skip(skip).Take(take)
                        .Select(tu => new TestUser {
                            Id = tu.Id,
                            UserIdentification = tu.UserIdentification,
                            Email = tu.Email,
                            CurrentQuestionOrder = tu.CurrentQuestionOrder,
                            DateTimeStart = tu.DateTimeStart,
                            DateTimeEnd = tu.DateTimeEnd,
                            DateTimeScheduled = tu.DateTimeScheduled,
                            NumberReviewers = tu.NumberReviewers,
                            NumberReviewerScores = tu.NumberReviewerScores,
                            Score = tu.Score,
                            NumberTimesRefreshed = tu.NumberTimesRefreshed,
                            Test = new Test { Title = tu.Test.Title, Id = tu.Test.Id }
                        }).ToListAsync();
                } else {
                    TestUser = await _context.TestUsers.Include(t => t.Test).Where(whereLambda)
                        .Where(tu => tu.DateTimeStart > date || tu.DateTimeScheduled > date || (tu.DateTimeStart == null && tu.DateTimeScheduled == null))
                        .Where(tu => search == "" || (tu.Email.Contains(search) || tu.UserIdentification.Contains(search)))
                        .Where(tu => testsearch == "" || tu.Test.Title.Contains(testsearch))
                        .OrderByDescending(tu => tu.DateTimeStart).Skip(skip).Take(take)
                        .Select(tu => new TestUser {
                            Id = tu.Id,
                            UserIdentification = tu.UserIdentification,
                            Email = tu.Email,
                            CurrentQuestionOrder = tu.CurrentQuestionOrder,
                            DateTimeStart = tu.DateTimeStart,
                            DateTimeEnd = tu.DateTimeEnd,
                            DateTimeScheduled = tu.DateTimeScheduled,
                            NumberReviewers = tu.NumberReviewers,
                            NumberReviewerScores = tu.NumberReviewerScores,
                            Score = tu.Score,
                            NumberTimesRefreshed = tu.NumberTimesRefreshed,
                            Test = new Test { Title = tu.Test.Title, Id = tu.Test.Id }
                        }).ToListAsync();
                }
            }
        }
    }
}