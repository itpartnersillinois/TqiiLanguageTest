using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class IsbeReportModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public IsbeReportModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<ReportIsbe> ReportIsbes { get; set; } = default!;

        public void OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.ReportIsbes != null) {
                ReportIsbes = _context.ReportIsbes.Where(ri => ri.FinalizedDate == null).ToList();
            }
        }
    }
}