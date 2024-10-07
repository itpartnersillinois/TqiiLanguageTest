using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Controllers {

    [Route("testrater")]
    public class TestRaterController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public TestRaterController(PermissionsHandler permissions, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult Index(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }

            var sb = new StringBuilder();

            if (_context.RaterNames != null && _context.TestUsers != null && _context.RaterTests != null) {
                var testinformation = _context.TestUsers.Include(t => t.Test).Select(tu => new { tu.Id, tu.UserIdentification, tu.Email, tu.DateTimeEnd, tu.Test.Title }).First(tu => tu.Id == id);
                sb.AppendLine("Email:\t" + testinformation.Email ?? "");
                sb.AppendLine("User ID:\t" + testinformation.UserIdentification ?? "");
                sb.AppendLine("Test:\t" + testinformation.Title ?? "");
                sb.AppendLine("Date Ended:\t" + testinformation.DateTimeEnd.ToString() ?? "");

                var raterInformation = _context.RaterAnswers.Include(ra => ra.RaterTest).ThenInclude(rt => rt.Rater).Include(ra => ra.Answer).ThenInclude(a => a.Question).Where(ra => ra.RaterTest.TestUserId == id && ra.Answer.TestUserId == testinformation.Id && ra.Answer.Question.QuestionType != QuestionEnum.Instructions).Select(ra => new { QuestionTitle = ra.Answer.Question.Title, ra.Score, ra.ScoreText, Notes = ra.Notes == "" ? "no notes" : ra.Notes, RaterName = ra.RaterTest.Rater.FullName, AnswerDate = ra.Answer.DateTimeEnd }).ToList();

                var answers = raterInformation.Select(ri => ri.QuestionTitle).Distinct();
                var raterNames = raterInformation.Select(ri => ri.RaterName).Distinct();

                sb.Append("Question");
                sb.Append('\t');
                sb.Append("Date Answered");

                foreach (var raterName in raterNames) {
                    sb.Append('\t');
                    sb.Append(raterName + " score");
                    sb.Append('\t');
                    sb.Append(raterName + " details");
                    sb.Append('\t');
                    sb.Append(raterName + " notes");
                }

                foreach (var answer in answers) {
                    sb.AppendLine("");
                    sb.Append(answer);
                    sb.Append('\t');
                    var targetAnswer = raterInformation.FirstOrDefault(t => t.QuestionTitle == answer);
                    if (targetAnswer != null && targetAnswer.AnswerDate.HasValue) {
                        sb.Append(targetAnswer.AnswerDate.Value.ToShortDateString() + " " + targetAnswer.AnswerDate.Value.ToShortTimeString());
                    }
                    foreach (var raterName in raterNames) {
                        var target = raterInformation.FirstOrDefault(t => t.QuestionTitle == answer && t.RaterName == raterName);
                        if (target != null) {
                            sb.Append('\t');
                            sb.Append(target.Score.ToString("0.00"));
                            sb.Append('\t');
                            sb.Append(target.ScoreText);
                            sb.Append('\t');
                            sb.Append(target.Notes);
                        }
                    }
                }
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", "tqii-rating-list.txt");
        }
    }
}