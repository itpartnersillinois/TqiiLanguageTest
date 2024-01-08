using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Controllers {

    [Route("testdownload")]
    public class TestDownloadController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public TestDownloadController(PermissionsHandler permissions, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
        }

        [HttpGet("all")]
        public IActionResult All() {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var testUsers = _context?.TestUsers.Include(tu => tu.Test).Where(tu => tu.Score > 0);

            var sb = new StringBuilder();

            sb.AppendLine("Test Information");
            foreach (var testUser in testUsers) {
                sb.Append(testUser.Email + '\t');
                sb.Append(testUser.UserIdentification + '\t');
                sb.Append(testUser.DateTimeStart.ToString() + '\t');
                sb.Append(testUser.DateTimeEnd.ToString() + '\t');
                sb.Append(testUser.Test.Title + '\t');
                sb.Append("Number of Reviewers: " + testUser.NumberReviewerScores.ToString() + '\t');
                sb.Append("Score " + testUser.Score.ToString() + '\t');
                sb.Append((testUser.IsPassed ? "Passed" : "Failed") + '\t');
                sb.Append(testUser.ReviewerNotes);
                sb.AppendLine();
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", "tqii-scores.txt");
        }

        [HttpGet("{id}")]
        public IActionResult Index(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var testUser = _context?.TestUsers?.SingleOrDefault(tu => tu.Id == id);
            if (testUser == null) {
                return NotFound();
            }
            var test = _context?.Tests?.Single(t => t.Id == testUser.TestId);
            if (test == null) {
                return NotFound();
            }
            var questions = _context?.Questions?.Where(q => q.TestId == testUser.TestId).Select(q => new { q.Id, q.Title }).ToList();
            var answers = _context?.Answers?.Where(a => a.TestUserId == id).OrderBy(a => a.DateTimeStart).Select(a => new { a.Id, a.QuestionId }).ToList();

            var answerIds = answers.Select(a => a.Id).ToList();

            var raters = _context?.RaterTests.Include(rt => rt.Rater).Where(rt => rt.TestUserId == id).ToList();

            var raterAnswers = _context?.RaterAnswers.Where(ra => answerIds.Contains(ra.AnswerId)).ToList();

            var sb = new StringBuilder();

            sb.AppendLine("Test Information");
            sb.Append(testUser.Email + '\t');
            sb.Append(testUser.UserIdentification + '\t');
            sb.Append(testUser.DateTimeStart.ToString() + '\t');
            sb.Append(testUser.DateTimeEnd.ToString() + '\t');
            sb.Append(test.Title + '\t');

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("Individual Answer Ratings");
            foreach (var answer in answers) {
                var question = questions?.SingleOrDefault(q => q.Id == answer.QuestionId);
                foreach (var raterAnswer in raterAnswers.Where(ra => ra.AnswerId == answer.Id)) {
                    var rater = raters.SingleOrDefault(r => r.Id == raterAnswer.RaterTestId);
                    sb.Append(question.Title + '\t');
                    sb.Append(rater.Rater.Email + '\t');
                    sb.Append(rater.IsExtraScorer ? "Extra" : "Regular" + '\t');
                    sb.Append("Score " + raterAnswer.Score.ToString() + '\t');
                    sb.Append(raterAnswer.Notes);
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
            sb.AppendLine("Final Ratings by Rater");
            foreach (var rater in raters) {
                sb.Append(rater.Rater.Email + '\t');
                sb.Append(rater.IsExtraScorer ? "Extra" : "Regular" + '\t');
                sb.Append("Score " + rater.FinalScore.ToString() + '\t');
                sb.Append(rater.Notes);
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine("Final Rating");
            sb.Append("Score " + testUser.Score.ToString() + '\t');
            sb.Append((testUser.IsPassed ? "Passed" : "Failed") + '\t');
            sb.Append(testUser.ReviewerNotes);
            sb.AppendLine();

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/txt", GenerateTitle(test, testUser));
        }

        private string GenerateTitle(Test test, TestUser testUser) {
            return $"{test.Title}-{(testUser.DateTimeStart.HasValue ? testUser.DateTimeStart.Value.ToString("yyyyMMdd") : "")}-{testUser.UserIdentification}-{testUser.Id}.txt";
        }
    }
}