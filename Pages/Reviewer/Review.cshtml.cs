using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Reviewer {

    public class ReviewModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public ReviewModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public Answer Answer { get; set; } = default!;

        public string AnswerId { get; set; }
        public IList<Tuple<int, string, int>> Answers { get; set; } = default!;

        public string Id { get; set; }

        public bool IsFinal { get; set; }
        public string NextAnswerId { get; set; }
        public string RaterId { get; set; }
        public string RaterNotes { get; set; }
        public int Rating { get; set; }

        public string UrlString { get; set; }

        public async Task OnGetAsync() {
            Rating = 9;
            Id = Request.Query["id"];
            RaterId = Request.Query["raterid"];
            AnswerId = Request.Query.ContainsKey("answerid") ? Request.Query["answerid"] : "0";
            IsFinal = Request.Query.ContainsKey("final");
            UrlString = $"review?id={Id}&raterid={RaterId}";
            var id = int.Parse(Id);
            var raterId = int.Parse(RaterId);
            var answerId = int.Parse(AnswerId);
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            Answers = new List<Tuple<int, string, int>>();

            if (_context.Answers != null) {
                var answerObjects = await _context.Answers.Include(a => a.Question).Where(a => a.TestUserId == id && a.Question.QuestionType != QuestionEnum.Instructions).OrderBy(a => a.DateTimeEnd).Select(a => new { a.Question.Title, a.Id }).ToListAsync();
                var nextAnswer = 0;
                for (var i = answerObjects.Count - 1; i >= 0; i--) {
                    if (nextAnswer == 0) {
                        Answers.Add(new Tuple<int, string, int>(answerObjects[i].Id, answerObjects[i].Title, nextAnswer));
                    } else {
                        Answers.Insert(0, new Tuple<int, string, int>(answerObjects[i].Id, answerObjects[i].Title, nextAnswer));
                    }
                    if (answerId == answerObjects[i].Id) {
                        NextAnswerId = nextAnswer.ToString();
                    }
                    nextAnswer = answerObjects[i].Id;
                }
            }

            if (_context.Answers != null && answerId != 0) {
                Answer = _context.Answers.Include(a => a.Question).Select(a => new Answer { Id = a.Id, QuestionId = a.QuestionId, BasicAnswers1 = a.BasicAnswers1, BasicAnswers2 = a.BasicAnswers2, BasicAnswers3 = a.BasicAnswers3, Text = a.Text, Question = new Question { Title = a.Question.Title, BasicQuestion1 = a.Question.BasicQuestion1, BasicQuestion2 = a.Question.BasicQuestion2, BasicQuestion3 = a.Question.BasicQuestion3, InteractiveReadingAnswer = a.Question.InteractiveReadingAnswer, QuestionText = a.Question.QuestionText, SentenceRepetionText = a.Question.SentenceRepetionText } }).First(a => a.Id == answerId);
                var raterAnswer = _context.RaterAnswers.FirstOrDefault(ra => ra.AnswerId == answerId && ra.RaterTestId == raterId);
                if (raterAnswer != null) {
                    RaterNotes = raterAnswer.Notes;
                    Rating = raterAnswer.Score;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (_context != null && _context.RaterAnswers != null) {
                Id = Request.Form["id"];
                RaterId = Request.Form["raterid"];
                var raterId = int.Parse(RaterId);
                // answering a single question
                if (Request.Form.ContainsKey("answerid")) {
                    AnswerId = Request.Form["answerid"];
                    NextAnswerId = Request.Form["nextid"];

                    if (Request.Form.ContainsKey("level")) {
                        var answerId = int.Parse(AnswerId);

                        var raterAnswer = _context.RaterAnswers.FirstOrDefault(ra => ra.AnswerId == answerId && ra.RaterTestId == raterId);
                        if (raterAnswer != null) {
                            raterAnswer.Notes = Request.Form["notes"];
                            raterAnswer.Score = int.Parse(Request.Form["level"]);
                            _context.RaterAnswers.Update(raterAnswer);
                        } else {
                            _context.RaterAnswers.Add(new RaterAnswer {
                                AnswerId = answerId,
                                DateFinished = DateTime.Now,
                                Notes = Request.Form["notes"],
                                Score = int.Parse(Request.Form["level"]),
                                RaterTestId = raterId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                    if (NextAnswerId == "0") {
                        return RedirectToPage("Review", new { id = Id, raterid = RaterId, final = "true" });
                    }
                    return RedirectToPage("Review", new { id = Id, raterid = RaterId, answerId = NextAnswerId });
                } else { //finalizing the test
                    var rater = _context.RaterTests.Single(rt => rt.Id == raterId);
                    rater.DateFinished = DateTime.Now;
                    var raterTotalScore = _context.RaterAnswers.Where(ra => ra.RaterTestId == raterId).Sum(ra => ra.Score);
                    var raterTotalAnswers = _context.RaterAnswers.Count(ra => ra.RaterTestId == raterId);
                    rater.FinalScore = (float) raterTotalScore / (float) raterTotalAnswers;
                    rater.Notes = Request.Form["notes"];
                    var id = int.Parse(Id);
                    var test = _context.TestUsers.Single(tu => tu.Id == id);
                    test.NumberReviewerScores++;
                    var raterName = _context.RaterNames.Single(rn => rn.Id == rater.RaterNameId);
                    raterName.NumberOfTests--;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage("Index");
        }

        public string UrlInfo(int answerid, int nextid) => answerid == 0 && nextid == 0 ? UrlString + "&final=true" : UrlString + "&answerid=" + answerid + "&nextid=" + nextid;
    }
}