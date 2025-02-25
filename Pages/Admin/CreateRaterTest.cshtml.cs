using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateRaterTestModel : PageModel {
        public readonly float ScorePassingRange = 2.2F;
        private readonly Autograding _autograding;
        private readonly LanguageDbContext _context;
        private readonly Finalizing _finalizing;
        private readonly PermissionsHandler _permissions;

        public CreateRaterTestModel(LanguageDbContext context, PermissionsHandler permissions, Autograding autograding, Finalizing finalizing) {
            _context = context;
            _permissions = permissions;
            _autograding = autograding;
            _finalizing = finalizing;
        }

        public IList<Tuple<string, string, string, int>> AssignedRaters { get; set; } = default!;
        public string DateEnded { get; set; } = default!;
        public string Email { get; set; } = default!;
        public float FinalScore { get; set; }
        public int Id { get; set; } = default!;

        public string IdString { get; set; } = default!;
        public bool IsFinalized { get; set; }
        public int NumberOfDiscrepencyQuestions { get; set; }
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

                var assignedRaterInformation = await _context.RaterTests.Include(rt => rt.Rater).Where(rt => rt.TestUserId == Id).Select(rt => new { rt.Id, rt.Rater.Email, rt.IsExtraScorer, rt.IsFinalScorer, rt.FinalScore, rt.DateFinished }).ToListAsync();

                AssignedRaters = assignedRaterInformation.Select(rt => new Tuple<string, string, string, int>(rt.Email, rt.IsExtraScorer ? " (Second Pass)" : rt.IsFinalScorer ? " (Final)" : "", rt.FinalScore == 0 ? "Not Scored" : "Final Score: " + rt.FinalScore.ToString("0.00"), rt.Id)).OrderBy(s => s.Item1).ToList();

                if (assignedRaterInformation.Any() && !assignedRaterInformation.Any(a => a.DateFinished == null)) {
                    FinalScore = assignedRaterInformation.Sum(a => a.FinalScore) / assignedRaterInformation.Count();
                }
                Raters = await _context.RaterNames.ToListAsync();
                Raters = Raters.Where(r => r.IsActive && !AssignedRaters.Select(ar => ar.Item1).Contains(r.Email)).ToList();

                NumberOfDiscrepencyQuestions = _context.RaterAnswers.Include(ra => ra.RaterTest).Where(ra => ra.RaterTest.TestUserId == Id && !ra.RaterTest.IsExtraScorer && !ra.RaterTest.IsFinalScorer)
                    .GroupBy(ra => ra.AnswerId)
                    .Where(rag => rag.Count() > 1 && rag.Max(r => r.Score) > ScorePassingRange && rag.Min(r => r.Score) < ScorePassingRange).Count();
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            var testUserId = int.Parse(Request.Form["id"].ToString());
            var raters = Request.Form.ContainsKey("raters") ? Request.Form["raters"].ToString() : "";
            var ratersSecond = Request.Form.ContainsKey("raters-second") ? Request.Form["raters-second"].ToString() : "";
            var testUser = _context.TestUsers.Include(tu => tu.Test).First(tu => tu.Id == testUserId);
            var currentDate = DateTime.Now;
            if (Request.Form.ContainsKey("finalize") && Request.Form.ContainsKey("adminreview")) {
                var currentEmail = User.Identity?.Name ?? "";
                var adminRater = _context.RaterNames.FirstOrDefault(rn => rn.Email == currentEmail);
                var finalRating = _context.RaterTests.FirstOrDefault(rt => rt.IsFinalScorer && rt.TestUserId == testUserId);
                if (adminRater != null) {
                    if (finalRating == null) {
                        var raterTest = new RaterTest { DateAssigned = currentDate, IsExtraScorer = false, IsFinalScorer = true, RaterNameId = adminRater.Id, TestUserId = testUserId, RaterAnswerRemoveIdString = "" };
                        var raterAnswersGrouping = _context.RaterAnswers.Include(ra => ra.RaterTest).Where(ra => ra.RaterTest.TestUserId == testUserId).ToList();
                        var raterAnswers = raterAnswersGrouping.GroupBy(ra => ra.AnswerId).Where(rag => rag.Count() > 1 && rag.Max(r => r.Score) - rag.Min(r => r.Score) < 2).ToArray();
                        var raterAnswerList = new List<RaterAnswer>();
                        foreach (var raterAnswer in raterAnswers) {
                            float score = 0;
                            float count = 0;
                            bool isSuspicious = true;
                            bool isDisqualified = true;
                            foreach (var answer in raterAnswer) {
                                score += answer.Score;
                                count++;
                                isDisqualified = isSuspicious && answer.IsDisqualified;
                                isSuspicious = isSuspicious && answer.IsSuspicious;
                            }
                            score = score / count;
                            raterAnswerList.Add(new RaterAnswer { AnswerId = raterAnswer.Key, DateFinished = currentDate, Score = score, RaterTestId = raterTest.Id, IsDisqualified = isDisqualified, IsSuspicious = isSuspicious, IsAnswered = true, Notes = "Autograded" });
                        }
                        raterTest.RaterAnswers = raterAnswerList;
                        _context.RaterTests.Add(raterTest);
                    } else {
                        finalRating.RaterNameId = adminRater.Id;
                    }
                }
            } else if (Request.Form.ContainsKey("finalize")) {
                testUser.ReviewerNotes = Request.Form["notes"];
                testUser.Score = float.Parse(Request.Form["score"].ToString());
            } else {
                int totals = 0;
                if (!string.IsNullOrWhiteSpace(raters)) {
                    foreach (var rater in raters.Split(',').Select(r => int.Parse(r.Trim()))) {
                        _context.RaterTests.Add(new RaterTest { DateAssigned = currentDate, IsExtraScorer = false, RaterNameId = rater, TestUserId = testUserId, RaterAnswerRemoveIdString = "" });
                        var rater1 = _context.RaterNames.First(rn => rn.Id == rater);
                        rater1.NumberOfTests++;
                        totals++;
                    }
                }
                if (!string.IsNullOrWhiteSpace(ratersSecond)) {
                    var removeItem = new List<int>();
                    var allAnswers = _context.RaterAnswers.Include(ra => ra.RaterTest).Where(ra => ra.RaterTest.TestUserId == testUserId).Select(ra => ra.AnswerId).Distinct().ToList();

                    var raterAnswers = new List<int>();
                    if (testUser.Test.TestType == TestEnum.SentenceRepetition) {
                        raterAnswers = _context.RaterAnswers.Include(ra => ra.RaterTest).Where(ra => ra.RaterTest.TestUserId == testUserId)
                            .GroupBy(ra => ra.AnswerId)
                            .Where(rag => rag.Count() > 1 && rag.Max(r => r.Score) - rag.Min(r => r.Score) > 1).Select(rag => rag.Key).ToList();
                    } else {
                        raterAnswers = _context.RaterAnswers.Include(ra => ra.RaterTest).Where(ra => ra.RaterTest.TestUserId == testUserId)
                            .GroupBy(ra => ra.AnswerId)
                            .Where(rag => rag.Count() > 1 && rag.Max(r => r.Score) > ScorePassingRange && rag.Min(r => r.Score) < ScorePassingRange).Select(rag => rag.Key).ToList();
                    }

                    var raterAnswerRemoveIdString = string.Join(',', allAnswers.Where(a => !raterAnswers.Contains(a)));

                    foreach (var rater in ratersSecond.Split(',').Select(r => int.Parse(r.Trim()))) {
                        _context.RaterTests.Add(new RaterTest { DateAssigned = currentDate, IsExtraScorer = true, RaterNameId = rater, TestUserId = testUserId, RaterAnswerRemoveIdString = raterAnswerRemoveIdString });
                        var rater2 = _context.RaterNames.First(rn => rn.Id == rater);
                        rater2.NumberOfTests++;
                        totals++;
                    }
                }
                testUser.NumberReviewers = testUser.NumberReviewers + totals;
            }

            _context.SaveChanges();
            if (Request.Form.ContainsKey("finalize") && Request.Form.ContainsKey("adminreview")) {
                var raterTest = _context.RaterTests.Where(rt => rt.TestUserId == testUserId && rt.IsFinalScorer).FirstOrDefault();
                if (raterTest != null) {
                    return Redirect($"/reviewer/review?id={testUserId}&raterid={raterTest.Id}");
                }
            }
            if (!Request.Form.ContainsKey("finalize") && !string.IsNullOrWhiteSpace(raters)) {
                _autograding.AutoGrade(currentDate, testUser, testUserId);
            }
            if (Request.Form.ContainsKey("finalize")) {
                _finalizing.FinalizeForTest(testUser);
            }

            return RedirectToPage("CreateRaterTest", new { id = testUserId });
        }
    }
}