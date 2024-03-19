using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("userdownload")]
    public class UserController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly LanguageDbContext _languageContext;
        private readonly PermissionsHandler _permissions;

        public UserController(PermissionsHandler permissions, ApplicationDbContext context, LanguageDbContext languageContext) {
            _context = context;
            _permissions = permissions;
            _languageContext = languageContext;
        }

        [HttpGet("all")]
        public IActionResult All() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var query = _languageContext.TestUsers.GroupBy(cm => cm.Email).Select(g => new { g.Key, MinDateTimeScheduled = g.Min(cm => cm.DateTimeScheduled) }).ToList();

            var queryTestsStarted = _languageContext.TestUsers.Include(tu => tu.Test).Where(tu => tu.Test != null && tu.Test.IsPractice && tu.DateTimeStart != null).Select(tu => tu.Email).Distinct().ToList();

            var queryTestsEnded = _languageContext.TestUsers.Include(tu => tu.Test).Where(tu => tu.Test != null && tu.Test.IsPractice && tu.DateTimeEnd != null).Select(tu => tu.Email).Distinct().ToList();

            var emailAndLanguage = _languageContext.Users.OrderByDescending(c => c.DateAdded).Select(c => new Tuple<string, string>(c.Email, c.Language)).ToList();

            var UsersWithTests = query.Select(g => new Tuple<string, DateTime?, string>(g.Key.Trim(), g.MinDateTimeScheduled, queryTestsEnded.Contains(g.Key) ? "Finished Practice Test" : queryTestsStarted.Contains(g.Key) ? "Started Practice Test" : "No Practice Test")).Distinct().OrderBy(c => c.Item2).ToList();

            var usersWithoutTests = _context.Users.Where(u => u.EmailConfirmed).OrderBy(u => u.NormalizedEmail).Select(u => u.NormalizedEmail.ToLowerInvariant()).ToList();
            var Users = new List<Tuple<string, string>>();

            foreach (var user in usersWithoutTests) {
                if (!UsersWithTests.Select(uwt => uwt.Item1).Contains(user)) {
                    if (emailAndLanguage.Select(e => e.Item1).Contains(user)) {
                        Users.Add(new Tuple<string, string>(user, emailAndLanguage.First(e => e.Item1 == user).Item2));
                    } else {
                        Users.Add(new Tuple<string, string>(user, "No language listed"));
                    }
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("User Information");
            foreach (var user in UsersWithTests) {
                sb.Append(user.Item1 + '\t');
                sb.Append(user.Item2.HasValue ? user.Item2?.ToString("g") : "Not scheduled");
                sb.Append('\t');
                sb.Append(user.Item3);
                sb.AppendLine();
            }
            sb.AppendLine("Other Information");
            foreach (var user in Users) {
                sb.Append(user.Item1 + '\t');
                sb.Append(user.Item2);
                sb.AppendLine();
            }
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", "tqii-list.txt");
        }
    }
}