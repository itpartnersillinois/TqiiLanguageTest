using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class Detail {
        public int AnswerId { get; set; }
        public string FinalNotes { get; set; }
        public int FinalScore { get; set; }
        public string Question { get; set; } = default!;
        public List<string> Raters { get; set; } = default!;
    }

    public class DetailReportModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public DetailReportModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<Detail> Details { get; set; } = default!;
        public IList<ReportDetail> ReportDetails { get; set; } = default!;

        public void OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var testUserId = int.Parse(Request.Query["id"]);

            if (_context.ReportDetails != null) {
                ReportDetails = _context.ReportDetails.Where(r => r.TestUserId == testUserId).ToList();
                Details = new List<Detail>();

                foreach (var reportDetail in ReportDetails.GroupBy(rd => rd.AnswerId)) {
                    Details.Add(new Detail {
                        AnswerId = reportDetail.Key,
                        FinalNotes = reportDetail.First().FinalIndividualNotes,
                        FinalScore = reportDetail.First().FinalIndividualScore,
                        Question = $"{reportDetail.First().QuestionName} ({reportDetail.First().QuestionType})",
                        Raters = reportDetail.Select(rd => $"{rd.RaterName}: {rd.RaterScore}").ToList(),
                    });
                }
            }
        }

        // https://localhost:7186/Admin/DetailReport?id=335
        public async Task<IActionResult> OnPostAsync() {
            var testUserId = int.Parse(Request.Query["id"]);
            foreach (var formInformation in Request.Form.Where(f => f.Key.StartsWith("notes-") || f.Key.StartsWith("score-"))) {
                var array = formInformation.Key.Split('-');
                var id = int.Parse(array[1]);
                if (array[0] == "notes") {
                    _context.ReportDetails.Where(r => r.AnswerId == id).ToList().ForEach(a => a.FinalIndividualNotes = formInformation.Value[0]);
                } else if (array[0] == "score") {
                    _context.ReportDetails.Where(r => r.AnswerId == id).ToList().ForEach(a => a.FinalIndividualScore = int.Parse(formInformation.Value[0]));
                }
                _context.SaveChanges();
            }
            return RedirectToPage("DetailReport", new { id = testUserId });
        }
    }
}