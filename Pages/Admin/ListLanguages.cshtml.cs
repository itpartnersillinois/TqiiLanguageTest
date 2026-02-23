using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListLanguagesModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ListLanguagesModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public int Id { get; set; }

        public IList<LanguageOptions> Languages { get; set; } = default!;

        [BindProperty]
        public string NewCharacters { get; set; } = default!;

        [BindProperty]
        public string NewLanguage { get; set; } = default!;

        [BindProperty]
        public bool NewPopup { get; set; }

        [BindProperty]
        public bool NewUseStrict { get; set; }

        public async Task OnGetAsync(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.LanguageOptions != null) {
                Languages = await _context.LanguageOptions.OrderBy(r => r.Language).ToListAsync();
            }
            if (id != 0) {
                Id = id;
                var language = Languages.FirstOrDefault(l => l.Id == id);
                if (language != null) {
                    NewCharacters = language.Characters;
                    NewLanguage = language.Language;
                    NewPopup = language.Popout;
                    NewUseStrict = language.EnforceStrictGrading;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            var item = await _context.LanguageOptions.FirstOrDefaultAsync(l => l.Id == Id);
            if (item != null) {
                item.Language = NewLanguage?.Trim() ?? "";
                item.Characters = NewCharacters?.Trim() ?? "";
                item.Popout = NewPopup;
                item.EnforceStrictGrading = NewUseStrict;
            } else {
                _context.LanguageOptions.Add(new LanguageOptions { Language = NewLanguage?.Trim() ?? "", Characters = NewCharacters?.Trim() ?? "", Popout = NewPopup, EnforceStrictGrading = NewUseStrict });
            }
            _ = await _context.SaveChangesAsync();
            return RedirectToPage("./ListLanguages");
        }
    }
}