using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListLoggedInUsersModel : PageModel {
        public List<string> Users;
        public List<Tuple<string, DateTime?>> UsersWithTests;
        private readonly ApplicationDbContext _context;
        private readonly LanguageDbContext _languageContext;

        public ListLoggedInUsersModel(ApplicationDbContext context, LanguageDbContext languageContext) {
            _context = context;
            _languageContext = languageContext;
            Users = new List<string>();
            UsersWithTests = new List<Tuple<string, DateTime?>>();
        }

        public void OnGet() {
            var query = _languageContext.TestUsers.GroupBy(cm => cm.Email).Select(g => new { g.Key, MinDateTimeScheduled = g.Min(cm => cm.DateTimeScheduled) }).ToList();
            UsersWithTests = query.Select(g => new Tuple<string, DateTime?>(g.Key, g.MinDateTimeScheduled)).Distinct().OrderBy(c => c.Item1).ToList();
            Users = _context.Users.Where(u => u.EmailConfirmed).OrderBy(u => u.NormalizedEmail).Select(u => u.NormalizedEmail.ToLowerInvariant()).ToList();
        }
    }
}