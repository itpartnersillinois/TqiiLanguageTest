using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Controllers {

    [Route("audioanswerdownload")]
    public class AudioDownloadController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public AudioDownloadController(PermissionsHandler permissions, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult Index(int id) {
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "") && !_permissions.IsAdmin(User.Identity?.Name ?? "")) {
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
            var questions = _context?.Questions?.Where(q => q.TestId == testUser.TestId).ToList();
            var answers = _context?.Answers?.Where(a => a.TestUserId == id).OrderBy(a => a.DateTimeStart).ToList() ?? new List<Answer>();
            using (var memoryStream = new MemoryStream()) {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
                    foreach (var answer in answers) {
                        var question = questions?.SingleOrDefault(q => q.Id == answer.QuestionId);
                        var prefix = GeneratePrefix(test, testUser, question, answer);
                        if (answer.Recording.Count() > 0) {
                            var file = archive.CreateEntry($"{prefix}_answer.wav");
                            using (var stream = file.Open()) {
                                stream.Write(answer.Recording, 0, answer.Recording.Length);
                            }
                        }
                        if (question != null) {
                            if (question.Recording.Count() > 0) {
                                var file = archive.CreateEntry($"{prefix}_question.wav");
                                using (var stream = file.Open()) {
                                    stream.Write(question.Recording, 0, question.Recording.Length);
                                }
                            }
                            var sb = new StringBuilder();
                            AddString(ref sb, "ID " + answer.Id, false);
                            AddString(ref sb, "Title: " + question.Title, false);
                            AddString(ref sb, "Started at " + answer.DateTimeStart?.ToString("G"), false);
                            AddString(ref sb, "Ended at " + answer.DateTimeEnd?.ToString("G"), false);
                            AddString(ref sb, "Duration: " + (answer.DateTimeEnd - answer.DateTimeStart)?.ToString("c"), false);
                            AddString(ref sb, question.IntroductionText, true);
                            AddString(ref sb, question.QuestionText, true);
                            AddString(ref sb, answer.Text, true);
                            AddString(ref sb, question.RecordingText, true);
                            AddString(ref sb, question.InteractiveReadingAnswer, true);
                            AddString(ref sb, answer.InteractiveReadingAnswer, true);
                            AddString(ref sb, question.BasicQuestion1, true);
                            AddString(ref sb, answer.BasicAnswers1, true);
                            AddString(ref sb, question.BasicQuestion2, true);
                            AddString(ref sb, answer.BasicAnswers2, true);
                            AddString(ref sb, question.BasicQuestion3, true);
                            AddString(ref sb, answer.BasicAnswers3, false);
                            AddString(ref sb, "------------------------", false);
                            var answerKey = archive.CreateEntry($"{prefix}_details.txt");
                            var questionList = Encoding.UTF8.GetBytes(sb.ToString());
                            using (var stream = answerKey.Open()) {
                                stream.Write(questionList, 0, questionList.Length);
                            }
                        }
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", GenerateTitle(test, testUser));
            };
        }

        [HttpGet("multiple/{idList}")]
        public IActionResult Multiple(string idList) {
            var ids = idList.Split('-').Select(i => int.Parse(i)).ToList();
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "") && !_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var testUsers = _context?.TestUsers.Include(tu => tu.Test).Where(tu => ids.Contains(tu.Id)).ToList();

            var questions = _context?.Questions?.Where(q => testUsers.Select(tu => tu.Test.Id).Contains(q.TestId)).ToList();
            var answers = _context?.Answers?.Where(a => ids.Contains(a.TestUserId ?? 0)).ToList() ?? new List<Answer>();
            using (var memoryStream = new MemoryStream()) {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
                    foreach (var answer in answers) {
                        var question = questions?.SingleOrDefault(q => q.Id == answer.QuestionId);
                        var prefix = GeneratePrefix(testUsers.Single(tu => tu.Id == answer.TestUserId).Test ?? new Test(), testUsers.Single(tu => tu.Id == answer.TestUserId), question, answer);
                        if (answer.Recording.Count() > 0) {
                            var file = archive.CreateEntry($"{prefix}_answer.wav");
                            using (var stream = file.Open()) {
                                stream.Write(answer.Recording, 0, answer.Recording.Length);
                            }
                        }
                        if (question != null) {
                            if (question.Recording.Count() > 0) {
                                var file = archive.CreateEntry($"{prefix}_question.wav");
                                using (var stream = file.Open()) {
                                    stream.Write(question.Recording, 0, question.Recording.Length);
                                }
                            }
                            var sb = new StringBuilder();
                            AddString(ref sb, "ID " + answer.Id, false);
                            AddString(ref sb, "Title: " + question.Title, false);
                            AddString(ref sb, "Started at " + answer.DateTimeStart?.ToString("G"), false);
                            AddString(ref sb, "Ended at " + answer.DateTimeEnd?.ToString("G"), false);
                            AddString(ref sb, "Duration: " + (answer.DateTimeEnd - answer.DateTimeStart)?.ToString("c"), false);
                            AddString(ref sb, question.IntroductionText, true);
                            AddString(ref sb, question.QuestionText, true);
                            AddString(ref sb, answer.Text, true);
                            AddString(ref sb, question.RecordingText, true);
                            AddString(ref sb, question.InteractiveReadingAnswer, true);
                            AddString(ref sb, answer.InteractiveReadingAnswer, true);
                            AddString(ref sb, question.BasicQuestion1, true);
                            AddString(ref sb, answer.BasicAnswers1, true);
                            AddString(ref sb, question.BasicQuestion2, true);
                            AddString(ref sb, answer.BasicAnswers2, true);
                            AddString(ref sb, question.BasicQuestion3, true);
                            AddString(ref sb, answer.BasicAnswers3, false);
                            AddString(ref sb, "------------------------", false);
                            var answerKey = archive.CreateEntry($"{prefix}_details.txt");
                            var questionList = Encoding.UTF8.GetBytes(sb.ToString());
                            using (var stream = answerKey.Open()) {
                                stream.Write(questionList, 0, questionList.Length);
                            }
                        }
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", "tqii.zip");
            };
        }

        private void AddString(ref StringBuilder sb, string value, bool lineBreak) {
            if (!string.IsNullOrWhiteSpace(value)) {
                _ = sb.AppendLine(value);
                if (lineBreak) {
                    _ = sb.AppendLine("---------");
                }
            }
        }

        private string GeneratePrefix(Test test, TestUser testUser, Question? question, Answer answer) {
            return $"{test.Title}-{(testUser.DateTimeStart.HasValue ? testUser.DateTimeStart.Value.ToString("yyyyMMdd") : "")}-{testUser.UserIdentification}-{question?.Title}-{answer.Id}";
        }

        private string GenerateTitle(Test test, TestUser testUser) {
            return $"{test.Title}-{(testUser.DateTimeStart.HasValue ? testUser.DateTimeStart.Value.ToString("yyyyMMdd") : "")}-{testUser.UserIdentification}-{testUser.Id}.zip";
        }
    }
}