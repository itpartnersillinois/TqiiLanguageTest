using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class ListRatingModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ListRatingModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public string DateEnded { get; set; }
        public string Email { get; set; }
        public string RaterEmail { get; set; }
        public List<Tuple<string, int, string>> RaterInformation { get; set; }
        public string RaterNotes { get; set; }
        public string TestName { get; set; }
        public string UserId { get; set; }

        public void OnGet() {
            var Id = int.Parse(Request.Query["id"]);
            var raterTestId = int.Parse(Request.Query["ratertestid"]);
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.RaterNames != null && _context.TestUsers != null && _context.RaterTests != null) {
                var testinformation = _context.TestUsers.Include(t => t.Test).Select(tu => new { tu.Id, tu.UserIdentification, tu.Email, tu.DateTimeEnd, tu.Test.Title }).First(tu => tu.Id == Id);
                Email = testinformation.Email ?? "";
                TestName = testinformation.Title ?? "";
                DateEnded = testinformation.DateTimeEnd.ToString() ?? "";
                UserId = testinformation.UserIdentification ?? "";

                RaterInformation = _context.RaterAnswers.Include(ra => ra.Answer).ThenInclude(a => a.Question).Where(ra => ra.RaterTestId == raterTestId && ra.Answer.TestUserId == testinformation.Id && ra.Answer.Question.QuestionType != QuestionEnum.Instructions).OrderBy(ra => ra.Answer.DateTimeEnd).Select(ra => new Tuple<string, int, string>(ra.Answer.Question.Title, ra.Score, ra.Notes == "" ? "(no notes)" : " (" + ra.Notes + ")")).ToList();

                var raterTest = _context.RaterTests.Include(rt => rt.Rater).Single(rt => rt.Id == raterTestId);
                RaterEmail = raterTest.Rater?.Email ?? "";
                RaterNotes = raterTest.Notes;
            }
        }
    }
}