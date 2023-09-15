using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.FileManager;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Controllers {

    [Route("audioanswerdownload")]
    public class AudioDownloadController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public AudioDownloadController(PermissionsHandler permissions, PackageHelper packageHelper, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
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

            var questions = _context?.Questions?.Where(q => q.TestId == testUser.TestId).ToList();

            var answers = _context?.Answers?.Where(a => a.TestUserId == id).OrderBy(a => a.DateTimeStart).ToList() ?? new List<Answer>();

            using (var memoryStream = new MemoryStream()) {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
                    foreach (var answer in answers) {
                        if (answer.Recording.Count() > 0) {
                            var file = archive.CreateEntry("tqii_" + answer.Id + "_answer.wav");
                            using (var stream = file.Open()) {
                                stream.Write(answer.Recording, 0, answer.Recording.Length);
                            }
                        }
                        var question = questions?.SingleOrDefault(q => q.Id == answer.QuestionId);
                        if (question != null) {
                            if (question.Recording.Count() > 0) {
                                var file = archive.CreateEntry("tqii_" + answer.Id + "_question.wav");
                                using (var stream = file.Open()) {
                                    stream.Write(question.Recording, 0, question.Recording.Length);
                                }
                            }
                            var sb = new StringBuilder();
                            AddString(ref sb, "ID " + answer.Id);
                            AddString(ref sb, question.Title);
                            AddString(ref sb, "Started at " + answer.DateTimeStart?.ToString("G"));
                            AddString(ref sb, "Ended at " + answer.DateTimeEnd?.ToString("G"));
                            AddString(ref sb, "Duration: " + (answer.DateTimeEnd - answer.DateTimeStart)?.ToString("c"));
                            AddString(ref sb, question.IntroductionText);
                            if (question.QuestionType == QuestionEnum.SentenceRepetition ||
                                question.QuestionType == QuestionEnum.IntegratedSpeaking) {
                                AddString(ref sb, question.QuestionText);
                                AddString(ref sb, answer.Text);
                                AddString(ref sb, question.RecordingText);
                            }
                            if (question.QuestionType == QuestionEnum.InteractiveReading ||
                                question.QuestionType == QuestionEnum.BasicQuestions) {
                                AddString(ref sb, question.InteractiveReadingAnswer);
                                AddString(ref sb, answer.InteractiveReadingAnswer);
                                AddString(ref sb, question.BasicQuestion1);
                                AddString(ref sb, answer.BasicAnswers1);
                                AddString(ref sb, question.BasicQuestion2);
                                AddString(ref sb, answer.BasicAnswers2);
                                AddString(ref sb, question.BasicQuestion3);
                                AddString(ref sb, answer.BasicAnswers3);
                            }
                            AddString(ref sb, "------------------------");
                            var answerKey = archive.CreateEntry("tqii_" + answer.Id + "_details.txt");
                            var questionList = Encoding.UTF8.GetBytes(sb.ToString());
                            using (var stream = answerKey.Open()) {
                                stream.Write(questionList, 0, questionList.Length);
                            }
                        }
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", "test_user_id_" + id + ".zip");
            };
        }

        private void AddString(ref StringBuilder sb, string value) {
            if (!string.IsNullOrWhiteSpace(value)) {
                _ = sb.AppendLine(value);
            }
        }
    }
}