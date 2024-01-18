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
        public string NewRaterEmail { get; set; } = default!;

        [BindProperty]
        public string NewRaterName { get; set; } = default!;

        [BindProperty]
        public string NewRaterNotes { get; set; } = default!;

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
            NewRaterName = string.IsNullOrWhiteSpace(NewRaterName) ? "" : NewRaterName;
            NewRaterNotes = string.IsNullOrWhiteSpace(NewRaterNotes) ? "" : NewRaterNotes;
            if (NewRaterEmail.Contains(',')) {
                var emailArray = NewRaterEmail.Split(',');
                var nameArray = NewRaterName.Split(',');
                for (int i = 0; i < emailArray.Length; i++) {
                    _context.RaterNames.Add(new RaterName {
                        Email = emailArray[i].Trim(),
                        Notes = NewRaterNotes.Trim(),
                        FullName = nameArray.Length < i && !string.IsNullOrWhiteSpace(nameArray[i]) ? nameArray[i].Trim() : "",
                        NumberOfTests = 0
                    });
                }
            } else {
                var item = await _context.RaterNames.FirstOrDefaultAsync(rn => rn.Email == NewRaterEmail.Trim());
                if (item != null) {
                    item.FullName = NewRaterName.Trim();
                    item.Notes = NewRaterNotes.Trim();
                    item.NumberOfTests = 0;
                } else {
                    _context.RaterNames.Add(new RaterName { Email = NewRaterEmail.Trim(), FullName = NewRaterName.Trim(), Notes = NewRaterNotes.Trim(), NumberOfTests = 0 });
                }
            }
            _ = await _context.SaveChangesAsync();
            return RedirectToPage("./CreateRater");
        }
    }
}