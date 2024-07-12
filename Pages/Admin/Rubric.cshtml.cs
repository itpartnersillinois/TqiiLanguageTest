using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class RubricModel : PageModel {
        private readonly LanguageDbContext _context = default!;
        private readonly PermissionsHandler _permissions = default!;

        public RubricModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public RaterScale RaterScale { get; set; } = default!;

        public void OnGet() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            var questionIdString = Request.Query["questionid"];
            var idString = Request.Query["id"];
            if (!string.IsNullOrWhiteSpace(idString)) {
                var id = int.Parse(idString);
                RaterScale = _context?.RaterScales?.Find(id) ?? throw new SecurityException("Rater Scale not found");
            } else if (!string.IsNullOrWhiteSpace(questionIdString)) {
                var questionId = int.Parse(questionIdString);
                var question = _context?.RaterScales?.Find(questionId) ?? throw new SecurityException("Rater Scale not found");
                RaterScale = new RaterScale {
                    RaterScaleName = question.RaterScaleName,
                    QuestionInformationId = questionId
                };
            } else {
                RedirectToPage("/Admin/RubricScale");
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            RaterScale.Description = RaterScale.Description ?? "";
            RaterScale.Descriptors = RaterScale.Descriptors ?? "";
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }
            if (RaterScale.Id != 0 && string.IsNullOrWhiteSpace(RaterScale.Title)) {
                _context.Remove(RaterScale);
                _context.SaveChanges();
            } else if (RaterScale.Id != 0) {
                _context.Update(RaterScale);
                _context.SaveChanges();
            } else {
                _context.Add(RaterScale);
                _context.SaveChanges();
            }

            var questionIdString = Request.Query["questionid"];

            return RedirectToPage("RubricQuestion", new { id = questionIdString });
        }
    }
}