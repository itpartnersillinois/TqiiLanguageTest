using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListLoggedInUsersModel : PageModel {
        public List<Tuple<string, string>> Users;
        public List<Tuple<string, DateTime?, string>> UsersWithTests;
        private readonly ApplicationDbContext _context;
        private readonly LanguageDbContext _languageContext;

        public ListLoggedInUsersModel(ApplicationDbContext context, LanguageDbContext languageContext) {
            _context = context;
            _languageContext = languageContext;
            Users = new List<Tuple<string, string>>();
            UsersWithTests = new List<Tuple<string, DateTime?, string>>();
        }

        public void OnGet() {
            var query = _languageContext.TestUsers.GroupBy(cm => cm.Email).Select(g => new { g.Key, MinDateTimeScheduled = g.Min(cm => cm.DateTimeScheduled) }).ToList();

            var queryTestsStarted = _languageContext.TestUsers.Include(tu => tu.Test).Where(tu => tu.Test != null && tu.Test.IsPractice && tu.DateTimeStart != null).Select(tu => tu.Email).Distinct().ToList();

            var queryTestsEnded = _languageContext.TestUsers.Include(tu => tu.Test).Where(tu => tu.Test != null && tu.Test.IsPractice && tu.DateTimeEnd != null).Select(tu => tu.Email).Distinct().ToList();

            var emailAndLanguage = _languageContext.Users.OrderByDescending(c => c.DateAdded).Select(c => new Tuple<string, string>(c.Email, c.Language)).ToList();

            UsersWithTests = query.Select(g => new Tuple<string, DateTime?, string>(g.Key.Trim(), g.MinDateTimeScheduled, queryTestsEnded.Contains(g.Key) ? "Finished Practice Test" : queryTestsStarted.Contains(g.Key) ? "Started Practice Test" : "No Practice Test")).Distinct().OrderBy(c => c.Item1).ToList();

            var usersWithoutTests = _context.Users.Where(u => u.EmailConfirmed).OrderBy(u => u.NormalizedEmail).Select(u => u.NormalizedEmail.ToLowerInvariant()).ToList();

            foreach (var user in usersWithoutTests) {
                if (emailAndLanguage.Select(e => e.Item1).Contains(user)) {
                    Users.Add(new Tuple<string, string>(user, emailAndLanguage.First(e => e.Item1 == user).Item2));
                } else {
                    Users.Add(new Tuple<string, string>(user, "No language listed"));
                }
            }
        }
    }
}