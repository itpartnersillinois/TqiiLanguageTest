using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListTestModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ListTestModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public string Language { get; set; } = "";

        public IList<Test> Test { get; set; } = default!;

        public async Task OnGetAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var languageList = _context.LanguageOptions?.Select(l => l.Language).ToList() ?? new List<string>();
            languageList.Insert(0, "");
            ViewData["Languages"] = new SelectList(languageList);
            Language = Request.Query["language"];
            if (_context.Tests != null) {
                Test = await _context.Tests.Where(t => t.Language == Language || Language == "").OrderBy(t => t.Language).ThenBy(t => t.IsPractice).ThenBy(t => t.Title).Select(t => new Test { Id = t.Id, Title = t.Title, Language = t.Language, NumberQuestions = t.NumberQuestions }).ToListAsync();
            }
        }
    }
}