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
        public int NewId { get; set; }

        [BindProperty]
        public string NewRaterEmail { get; set; } = default!;

        [BindProperty]
        public string NewRaterName { get; set; } = default!;

        [BindProperty]
        public string NewRaterNotes { get; set; } = default!;

        public IList<RaterName> Raters { get; set; } = default!;

        public Dictionary<int, int> RaterTestCount { get; set; } = default!;

        public async Task OnGetAsync(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.RaterNames != null && _context.RaterTests != null) {
                Raters = await _context.RaterNames.Where(r => r.IsActive).OrderBy(r => r.Email).ToListAsync();
                RaterTestCount = _context.RaterTests.Where(r => r.DateFinished != null).GroupBy(r => r.RaterNameId).Select(rt => new { Id = rt.Key, Count = rt.Count() }).ToDictionary(a => a.Id, b => b.Count);
            }
            NewId = id;
            if (id != 0) {
                NewRaterEmail = Raters.FirstOrDefault(p => p.Id == id)?.Email ?? "";
                NewRaterName = Raters.FirstOrDefault(p => p.Id == id)?.FullName ?? "";
                NewRaterNotes = Raters.FirstOrDefault(p => p.Id == id)?.Notes ?? "";
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            NewRaterName = string.IsNullOrWhiteSpace(NewRaterName) ? "" : NewRaterName;
            NewRaterNotes = string.IsNullOrWhiteSpace(NewRaterNotes) ? "" : NewRaterNotes;
            if (NewId != 0) {
                if (string.IsNullOrWhiteSpace(NewRaterEmail)) {
                    var baseRater = _context.RaterNames.First(t => t.Id == NewId);
                    baseRater.IsActive = false;
                    _context.RaterNames.Update(baseRater);
                } else {
                    var baseRater = _context.RaterNames.AsNoTracking().First(t => t.Id == NewId);
                    baseRater.Email = NewRaterEmail;
                    baseRater.FullName = NewRaterName;
                    baseRater.Notes = NewRaterNotes;
                    baseRater.IsActive = true;
                    _context.RaterNames.Update(baseRater);
                }
            } else if (NewRaterEmail.Contains(',')) {
                var emailArray = NewRaterEmail.Split(',');
                var nameArray = NewRaterName.Split(',');
                for (int i = 0; i < emailArray.Length; i++) {
                    _context.RaterNames.Add(new RaterName {
                        Email = emailArray[i].Trim(),
                        Notes = NewRaterNotes.Trim(),
                        FullName = nameArray.Length < i && !string.IsNullOrWhiteSpace(nameArray[i]) ? nameArray[i].Trim() : "",
                        NumberOfTests = 0,
                        IsActive = true
                    });
                }
            } else {
                var item = await _context.RaterNames.FirstOrDefaultAsync(rn => rn.Email == NewRaterEmail.Trim());
                if (item != null) {
                    item.FullName = NewRaterName.Trim();
                    item.Notes = NewRaterNotes.Trim();
                    item.NumberOfTests = 0;
                } else {
                    _context.RaterNames.Add(new RaterName { Email = NewRaterEmail.Trim(), FullName = NewRaterName.Trim(), Notes = NewRaterNotes.Trim(), NumberOfTests = 0, IsActive = true });
                }
            }
            _ = await _context.SaveChangesAsync();
            return RedirectToPage("./CreateRater");
        }
    }
}