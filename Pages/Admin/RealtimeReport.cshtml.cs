using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class RealtimeReportModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public RealtimeReportModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public List<Answer> AnswerList { get; set; } = default!;
        public int CurrentQuestion { get; set; }
        public string Email { get; set; } = default!;
        public int Id { get; set; } = default!;
        public string TestName { get; set; } = default!;
        public string UserId { get; set; } = default!;

        public async Task OnGetAsync() {
            var Id = int.Parse(Request.Query["id"]);
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.TestUsers != null) {
                var testinformation = _context.TestUsers.Include(tu => tu.Test).Select(tu => new { tu.Id, tu.UserIdentification, tu.Email, tu.DateTimeEnd, tu.Test.Title, tu.CurrentQuestionOrder }).First(tu => tu.Id == Id);
                Email = testinformation.Email ?? "";
                TestName = testinformation.Title ?? "";
                UserId = testinformation.UserIdentification ?? "";
                CurrentQuestion = testinformation.CurrentQuestionOrder;

                AnswerList = _context.Answers.Include(a => a.Question).Where(a => a.TestUserId == Id).OrderByDescending(a => a.DateTimeStart).Select(a => new Answer { DateTimeStart = a.DateTimeStart, DateTimeEnd = a.DateTimeEnd, QuestionText = a.Question.QuestionText, Text = a.Question.Title, NumberTimesRefreshed = a.NumberTimesRefreshed }).ToList();
            }
        }
    }
}