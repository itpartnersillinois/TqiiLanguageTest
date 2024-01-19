using System.Text;
using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("reportdownload")]
    public class ReportController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ReportController(PermissionsHandler permissions, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
        }

        [HttpGet("detail")]
        public IActionResult Detail() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var report = _context?.ReportDetails.Where(ri => ri.TotalScore > 0);

            var sb = new StringBuilder();

            sb.AppendLine("Detail Report");

            sb.Append("User Identification" + '\t');
            sb.Append("Email" + '\t');
            sb.Append("Test Date" + '\t');
            sb.Append("Test Time" + '\t');
            sb.Append("Test Name" + '\t');
            sb.Append("Question" + '\t');
            sb.Append("Task" + '\t');
            sb.Append("Date Answered" + '\t');
            sb.Append("Response" + '\t');
            sb.Append("Answer Key" + '\t');
            sb.Append("Autograded Response" + '\t');
            sb.Append("Rater Name" + '\t');
            sb.Append("Rater Score" + '\t');
            sb.Append("Rater Comment");
            sb.AppendLine();

            foreach (var item in report) {
                sb.Append(item.UserIdentification + '\t');
                sb.Append(item.Email + '\t');
                sb.Append(item.TestDate.ToShortDateString() + '\t');
                sb.Append(item.TestDate.ToShortTimeString() + '\t');
                sb.Append(item.TestName + '\t');
                sb.Append(item.QuestionName + '\t');
                sb.Append(item.QuestionType + '\t');
                sb.Append(item.QuestionAnswered.ToShortTimeString() + '\t');
                sb.Append(item.Answer + '\t');
                sb.Append(item.AnswerKey + '\t');
                sb.Append(item.AutogradedScore + '\t');
                sb.Append(item.RaterName + '\t');
                sb.Append(item.RaterScore.ToString() + '\t');
                sb.Append(item.RaterNotes);
                sb.AppendLine();
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", "tqii-report-detail.txt");
        }

        [HttpGet("isbe")]
        public IActionResult Isbe() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var report = _context?.ReportIsbes.Where(ri => ri.FinalizedDate != null);

            var sb = new StringBuilder();

            sb.AppendLine("ISBE Report");

            sb.Append("User Identification" + '\t');
            sb.Append("Email" + '\t');
            sb.Append("Last Test Date" + '\t');
            sb.Append("Last Test Time" + '\t');
            sb.Append("Last Test Name" + '\t');
            sb.Append("Total Score" + '\t');
            sb.Append("Sentence Repetition Score" + '\t');
            sb.Append("Integrated Speaking Score" + '\t');
            sb.Append("Interactive Reading Score" + '\t');
            sb.Append("Decision");
            sb.AppendLine();

            foreach (var item in report) {
                sb.Append(item.UserIdentification + '\t');
                sb.Append(item.Email + '\t');
                sb.Append(item.TestDate.ToShortDateString() + '\t');
                sb.Append(item.TestDate.ToShortTimeString() + '\t');
                sb.Append(item.TestName + '\t');
                sb.Append(item.TotalScore.ToString() + '\t');
                sb.Append(item.SentenceRepetitionScore.ToString() + '\t');
                sb.Append(item.IntegratedSpeakingScore.ToString() + '\t');
                sb.Append(item.InteractiveReadingScore.ToString() + '\t');
                sb.Append(item.IsPassed ? "Awarded certificate" : "Denied certificate");
                sb.AppendLine();
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", "tqii-report-isbe.txt");
        }

        [HttpGet("fail/{id}")]
        public IActionResult ReportFail(int id) => ProcessReport(id, false);

        [HttpGet("pass/{id}")]
        public IActionResult ReportPass(int id) => ProcessReport(id, true);

        private IActionResult ProcessReport(int id, bool pass) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var report = _context?.ReportIsbes?.SingleOrDefault(ri => ri.Id == id);
            if (report == null) {
                return NotFound();
            }
            report.FinalizedDate = DateTime.Now;
            report.IsPassed = pass;
            _context?.SaveChanges();
            return Ok();
        }
    }
}