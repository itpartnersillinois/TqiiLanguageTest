using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class RubricQuestionModel : PageModel {
        private readonly LanguageDbContext _context = default!;
        private readonly PermissionsHandler _permissions = default!;

        public RubricQuestionModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public int QuestionId { get; set; } = default!;

        [BindProperty]
        public RaterScale RaterScale { get; set; } = default!;

        public Dictionary<int, string> RaterScaleAnswers { get; set; } = default!;

        public string RaterScaleName { get; set; } = default!;

        public void OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            RaterScaleName = Request.Query["name"];
            var idString = Request.Query["id"];
            if (string.IsNullOrEmpty(RaterScaleName) && string.IsNullOrWhiteSpace(idString)) {
                Response.Redirect("/Admin/RubricScale");
            } else if (!string.IsNullOrWhiteSpace(idString)) {
                QuestionId = int.Parse(idString);
                RaterScale = _context?.RaterScales?.Find(QuestionId) ?? new RaterScale();
                if (RaterScale.Id == 0) {
                    Response.Redirect("/Admin/RubricScale");
                }
                RaterScaleAnswers = _context?.RaterScales?.Where(rs => rs.QuestionInformationId == QuestionId).OrderBy(rs => rs.Value).ToDictionary(rs => rs.Id, rs => rs.Title + " (" + rs.Value + ")") ?? new Dictionary<int, string>();
                RaterScaleName = RaterScale?.RaterScaleName ?? "";
            } else if (!string.IsNullOrWhiteSpace(RaterScaleName)) {
                var items = _context?.RaterScales?.Where(rs => rs.RaterScaleName == RaterScaleName && rs.QuestionInformationId == null).OrderBy(rs => rs.Order).ToDictionary(rs => rs.Id, rs => rs.Title) ?? new Dictionary<int, string>();
                items.Add(0, " -- choose a category -- ");
                ViewData["RaterScaleQuestions"] = new SelectList(items.OrderBy(s => s.Key).Select(s => new SelectListItem(s.Value, s.Key.ToString())), "Value", "Text");
                RaterScale = new RaterScale();
            } else {
                Response.Redirect("/Admin/RubricScale");
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            RaterScaleName = Request.Query["name"];
            RaterScale.Description = RaterScale.Description ?? "";
            RaterScale.Descriptors = "";

            if (RaterScale.Id != 0 && string.IsNullOrWhiteSpace(RaterScale.Title)) {
                foreach (var answer in _context.RaterScales?.Where(rs => rs.QuestionInformationId == RaterScale.Id).ToList() ?? new List<RaterScale>()) {
                    _context.Remove(answer);
                }
                _context.Remove(RaterScale);
                _context.SaveChanges();
                RedirectToPage("/Admin/RubricScale");
            } else if (RaterScale.Id != 0) {
                RaterScaleName = RaterScale.RaterScaleName;
                _context.Update(RaterScale);
                _context.SaveChanges();
            } else {
                RaterScale.RaterScaleName = RaterScaleName;
                _context.Add(RaterScale);
                _context.SaveChanges();
            }
            return RedirectToPage("/Admin/RubricQuestion", new { name = RaterScaleName });
        }
    }
}