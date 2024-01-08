using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateRaterModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public CreateRaterModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public string NewRaterName { get; set; } = default!;

        public IList<RaterName> Raters { get; set; } = default!;

        public async Task OnGetAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.RaterNames != null) {
                Raters = await _context.RaterNames.OrderBy(r => r.Email).ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (NewRaterName.Contains(',')) {
                var emailArray = NewRaterName.Split(',');
                foreach (var email in emailArray) {
                    _context.RaterNames.Add(new RaterName {
                        Email = email.Trim()
                    });
                }
            } else {
                _context.RaterNames.Add(new RaterName { Email = NewRaterName });
            }
            _context.SaveChanges();
            return RedirectToPage("./CreateRater");
        }
    }
}