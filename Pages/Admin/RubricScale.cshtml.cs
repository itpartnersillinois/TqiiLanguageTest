using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Pages.Admin {

    public class RubricScaleModel : PageModel {
        private readonly LanguageDbContext _context = default!;
        private readonly PermissionsHandler _permissions = default!;

        public RubricScaleModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public void OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var values = _context?.RaterScales?.Select(rs => rs.RaterScaleName).Distinct().ToList() ?? new List<string>();
            values.Add(" -- choose a scale -- ");
            ViewData["RaterScale"] = new SelectList(values.Select(s => s).OrderBy(d => d));
        }
    }
}