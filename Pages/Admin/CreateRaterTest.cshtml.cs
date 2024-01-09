using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateRaterTestModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public CreateRaterTestModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<Tuple<string, string, string, int>> AssignedRaters { get; set; } = default!;
        public string DateEnded { get; set; } = default!;
        public string Email { get; set; } = default!;
        public float FinalScore { get; set; }
        public int Id { get; set; } = default!;

        public string IdString { get; set; } = default!;
        public bool IsFinalized { get; set; }
        public IList<RaterName> Raters { get; set; } = default!;
        public string TestName { get; set; } = default!;
        public string UserId { get; set; } = default!;

        public async Task OnGetAsync() {
            var Id = int.Parse(Request.Query["id"]);
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            IsFinalized = Request.Query.ContainsKey("finalize");

            if (_context.RaterNames != null && _context.TestUsers != null && _context.RaterTests != null) {
                var testinformation = _context.TestUsers.Include(t => t.Test).Select(tu => new { tu.Id, tu.UserIdentification, tu.Email, tu.DateTimeEnd, tu.Test.Title }).First(tu => tu.Id == Id);
                Email = testinformation.Email ?? "";
                TestName = testinformation.Title ?? "";
                DateEnded = testinformation.DateTimeEnd.ToString() ?? "";
                UserId = testinformation.UserIdentification ?? "";
                IdString = Id.ToString();

                var assignedRaterInformation = await _context.RaterTests.Include(rt => rt.Rater).Where(rt => rt.TestUserId == Id).Select(rt => new { rt.Id, rt.Rater.Email, rt.IsExtraScorer, rt.FinalScore, rt.DateFinished }).ToListAsync();

                AssignedRaters = assignedRaterInformation.Select(rt => new Tuple<string, string, string, int>(rt.Email, rt.IsExtraScorer ? " (Second Pass)" : "", rt.FinalScore == 0 ? "Not Scored" : "Final Score: " + rt.FinalScore, rt.Id)).OrderBy(s => s.Item1).ToList();

                if (assignedRaterInformation.Any() && !assignedRaterInformation.Any(a => a.DateFinished == null)) {
                    FinalScore = assignedRaterInformation.Sum(a => a.FinalScore) / assignedRaterInformation.Count();
                }
                Raters = await _context.RaterNames.ToListAsync();
                Raters = Raters.Where(r => !AssignedRaters.Select(ar => ar.Item1).Contains(r.Email)).ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            var testUserId = int.Parse(Request.Form["id"].ToString());
            var raters = Request.Form.ContainsKey("raters") ? Request.Form["raters"].ToString() : "";
            var ratersSecond = Request.Form.ContainsKey("raters-second") ? Request.Form["raters-second"].ToString() : "";
            var testUser = _context.TestUsers.First(tu => tu.Id == testUserId);
            var currentDate = DateTime.Now;
            if (Request.Form.ContainsKey("finalize")) {
                testUser.ReviewerNotes = Request.Form["notes"];
                testUser.Score = float.Parse(Request.Form["score"].ToString());
            } else {
                int totals = 0;
                if (!string.IsNullOrWhiteSpace(raters)) {
                    foreach (var rater in raters.Split(',').Select(r => int.Parse(r.Trim()))) {
                        _context.RaterTests.Add(new RaterTest { DateAssigned = currentDate, IsExtraScorer = false, RaterNameId = rater, TestUserId = testUserId });
                        var rater1 = _context.RaterNames.First(rn => rn.Id == rater);
                        rater1.NumberOfTests++;
                        totals++;
                    }
                }
                if (!string.IsNullOrWhiteSpace(ratersSecond)) {
                    foreach (var rater in ratersSecond.Split(',').Select(r => int.Parse(r.Trim()))) {
                        _context.RaterTests.Add(new RaterTest { DateAssigned = currentDate, IsExtraScorer = true, RaterNameId = rater, TestUserId = testUserId });
                        var rater2 = _context.RaterNames.First(rn => rn.Id == rater);
                        rater2.NumberOfTests++;
                        totals++;
                    }
                }
                testUser.NumberReviewers = testUser.NumberReviewers + totals;
            }

            _context.SaveChanges();

            // TODO Need to extract information into separate component

            var autogradedQuestions = _context.Questions.Where(q => q.TestId == testUser.TestId && q.InteractiveReadingOptionsAnswerKey != "").Select(q => new { q.Id, q.InteractiveReadingOptionsAnswerKey }).ToList();
            var autogradedQuestionsBasedOnQuestions = _context.Questions.Where(q => q.TestId == testUser.TestId && q.BasicAnswerKey1 != "").Select(q => new { q.Id, q.BasicAnswerKey1, q.BasicAnswerKey2, q.BasicAnswerKey3 }).ToList();
            if (!Request.Form.ContainsKey("finalize") && (autogradedQuestions.Any() || autogradedQuestionsBasedOnQuestions.Any()) && !string.IsNullOrWhiteSpace(raters)) {
                var autograders = _context.RaterTests.Where(rt => rt.TestUserId == testUserId && rt.DateAssigned == currentDate);
                var answers = _context.Answers.Where(a => a.TestUserId == testUserId).Select(a => new { a.QuestionId, a.Id, a.Text, a.BasicAnswers1, a.BasicAnswers2, a.BasicAnswers3 }).ToList();
                foreach (var autogradeQuestion in autogradedQuestions) {
                    var answer = answers.SingleOrDefault(a => (a.QuestionId ?? 0) == autogradeQuestion.Id);
                    if (answer != null) {
                        foreach (var autograder in autograders) {
                            _context.RaterAnswers.Add(new RaterAnswer {
                                Score = answer != null && answer.Text.Contains(autogradeQuestion.InteractiveReadingOptionsAnswerKey) ? 1 : 0,
                                AnswerId = answer.Id,
                                DateFinished = currentDate,
                                Notes = "autograded",
                                RaterTestId = autograder.Id,
                            });
                        }
                    }
                }

                foreach (var autogradeQuestion in autogradedQuestionsBasedOnQuestions) {
                    var answer = answers.SingleOrDefault(a => (a.QuestionId ?? 0) == autogradeQuestion.Id);
                    int total = 0;
                    int count = 0;
                    if (autogradeQuestion.BasicAnswerKey1 != "") {
                        count++;
                        total = answer != null && answer.BasicAnswers1 == autogradeQuestion.BasicAnswerKey1 ? total + 1 : total;
                    }
                    if (autogradeQuestion.BasicAnswerKey2 != "") {
                        count++;
                        total = answer != null && answer.BasicAnswers2 == autogradeQuestion.BasicAnswerKey2 ? total + 1 : total;
                    }
                    if (autogradeQuestion.BasicAnswerKey3 != "") {
                        count++;
                        total = answer != null && answer.BasicAnswers3 == autogradeQuestion.BasicAnswerKey3 ? total + 1 : total + 1;
                    }

                    if (answer != null) {
                        foreach (var autograder in autograders) {
                            _context.RaterAnswers.Add(new RaterAnswer {
                                Score = total * 100 / count,
                                AnswerId = answer.Id,
                                DateFinished = currentDate,
                                Notes = "autograded",
                                RaterTestId = autograder.Id,
                            });
                        }
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToPage("CreateRaterTest", new { id = testUserId });
        }
    }
}