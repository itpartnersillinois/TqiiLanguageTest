using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Pages.Admin {

    public class DuplicateTestModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public DuplicateTestModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IActionResult OnGet(int id) {
            if (!_permissions.IsItemWriter(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            _context.Database.ExecuteSqlRaw("EXEC DuplicateTest {0}", id);
            return Redirect("/Admin/ListTest");
        }
    }
}