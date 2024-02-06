using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Reviewer {

    public class IndexModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public IndexModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<RaterTest> RaterTest { get; set; } = default!;

        public async Task OnGetAsync() {
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            var email = User.Identity?.Name;
            if (_context.TestUsers != null && email != "") {
                RaterTest = await _context.RaterTests.Include(rt => rt.Rater).Include(rt => rt.Test).ThenInclude(t => t.Test).Where(rt => rt.Rater.Email == email && rt.DateFinished == null).OrderBy(rt => rt.DateAssigned).Select(rt => new RaterTest { Id = rt.Id, Test = new TestUser { Id = rt.Test.Id, UserIdentification = rt.Test.UserIdentification, Email = rt.Test.Email, ReviewerNotes = rt.Test.Test.Title } }).ToListAsync();
            }
        }
    }
}