using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListLoggedInUsersModel : PageModel {
        public List<string> Users;
        public List<string> UsersWithTests;
        private readonly ApplicationDbContext _context;
        private readonly LanguageDbContext _languageContext;

        public ListLoggedInUsersModel(ApplicationDbContext context, LanguageDbContext languageContext) {
            _context = context;
            _languageContext = languageContext;
            Users = new List<string>();
        }

        public void OnGet() {
            Users = _context.Users.Where(u => u.EmailConfirmed).OrderBy(u => u.NormalizedEmail).Select(u => u.NormalizedEmail.ToLowerInvariant()).ToList();
            UsersWithTests = _languageContext.TestUsers.Select(tu => tu.Email.ToLowerInvariant()).Distinct().ToList();
        }
    }
}