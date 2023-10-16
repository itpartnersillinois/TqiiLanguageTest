using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Reviewer {

    public class TestUserModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public TestUserModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<Answer> Answers { get; set; } = default!;

        public async Task OnGetAsync(int id) {
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.TestUsers != null) {
                Answers = await _context.Answers.Include(a => a.Question).Where(a => a.TestUserId == id).OrderBy(a => a.DateTimeEnd).ToListAsync();
                var order = 0;
                foreach (var a in Answers) {
                    order++;
                    a.OrderBy = order;
                }
            }
        }
    }
}