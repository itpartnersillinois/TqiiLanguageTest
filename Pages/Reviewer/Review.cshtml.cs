using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;
using TqiiLanguageTest.RubricThinObjects;

namespace TqiiLanguageTest.Pages.Reviewer {

    public class ReviewModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly Finalizing _finalizing;
        private readonly PermissionsHandler _permissions;

        public ReviewModel(LanguageDbContext context, PermissionsHandler permissions, Finalizing finalizing) {
            _context = context;
            _permissions = permissions;
            _finalizing = finalizing;
        }

        public Answer Answer { get; set; } = default!;

        public string AnswerId { get; set; }
        public IList<Tuple<int, string, int, bool, bool>> Answers { get; set; } = default!;
        public IList<Tuple<string, string, string>> AssignedRaters { get; set; } = default!;

        public bool CanFinalize { get; set; }
        public string Id { get; set; }
        public bool IsDisqualified { get; set; }
        public bool IsFinal { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsSuspicious { get; set; }
        public string NextAnswerId { get; set; }
        public string NumberAnswered { get; set; }
        public List<RaterAnswer> OtherAnswerList { get; set; } = default!;
        public List<RaterAnswer> OtherAnswerListFinal { get; set; } = default!;
        public string RaterId { get; set; }
        public string RaterNotes { get; set; }
        public float Rating { get; set; }
        public string RatingAnswers { get; set; }
        public List<RubricThinQuestion> RubricThinQuestions { get; set; } = default!;
        public string UrlString { get; set; }

        public async Task OnGetAsync() {
            Rating = -1;
            Id = Request.Query["id"];
            RaterId = Request.Query["raterid"];
            AnswerId = Request.Query.ContainsKey("answerid") ? Request.Query["answerid"] : "0";
            IsFinal = Request.Query.ContainsKey("final");
            UrlString = $"review?id={Id}&raterid={RaterId}";
            var isFinalScorer = false;
            var id = int.Parse(Id);
            var raterId = int.Parse(RaterId);
            var answerId = int.Parse(AnswerId);
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            Answers = new List<Tuple<int, string, int, bool, bool>>();

            if (_context.Answers != null && _context.RaterAnswers != null && _context.RaterTests != null) {
                var answerObjects = await _context.Answers.Include(a => a.Question).Where(a => a.TestUserId == id && a.Question.QuestionType != QuestionEnum.Instructions).OrderBy(a => a.DateTimeEnd).Select(a => new { a.Question.Title, a.Id }).ToListAsync();
                var raterAnswers = await _context.RaterAnswers.Where(ra => ra.RaterTestId == raterId).Select(ra => ra.AnswerId).ToListAsync();
                var raterFlagged = await _context.RaterAnswers.Where(ra => ra.RaterTestId == raterId && ra.IsFlagged).Select(ra => ra.AnswerId).ToListAsync();
                var raterTest = _context.RaterTests.First(rt => rt.Id == raterId);
                var secondaryRater = raterTest.RaterAnswerRemoveIdString;
                isFinalScorer = raterTest.IsFinalScorer;
                if (!string.IsNullOrWhiteSpace(secondaryRater)) {
                    var answersToRemove = secondaryRater.Split(',').Select(i => int.Parse(i));
                    answerObjects.RemoveAll(a => answersToRemove.Contains(a.Id));
                }
                var nextAnswer = 0;
                for (var i = answerObjects.Count - 1; i >= 0; i--) {
                    var isAnswered = raterAnswers.Contains(answerObjects[i].Id);
                    var isFlagged = raterFlagged.Contains(answerObjects[i].Id);
                    if (nextAnswer == 0) {
                        Answers.Add(new Tuple<int, string, int, bool, bool>(answerObjects[i].Id, answerObjects[i].Title, nextAnswer, isAnswered, isFlagged));
                    } else {
                        Answers.Insert(0, new Tuple<int, string, int, bool, bool>(answerObjects[i].Id, answerObjects[i].Title, nextAnswer, isAnswered, isFlagged));
                    }
                    if (answerId == answerObjects[i].Id) {
                        NextAnswerId = nextAnswer.ToString();
                    }
                    nextAnswer = answerObjects[i].Id;
                }

                NumberAnswered = $"{raterAnswers.Count} / {answerObjects.Count}";
                CanFinalize = raterAnswers.Count == answerObjects.Count;
            }

            var scoreText = "";

            if (_context.Answers != null && answerId != 0) {
                Answer = _context.Answers.Include(a => a.Question).Select(a => new Answer { Id = a.Id, QuestionId = a.QuestionId, BasicAnswers1 = a.BasicAnswers1, BasicAnswers2 = a.BasicAnswers2, BasicAnswers3 = a.BasicAnswers3, Text = a.Text, NumberTimesRefreshed = a.NumberTimesRefreshed, QuestionType = a.Question.QuestionType, QuestionGuid = a.Question.Guid, Question = new Question { Title = a.Question.Title, BasicQuestion1 = a.Question.BasicQuestion1, BasicQuestion2 = a.Question.BasicQuestion2, BasicQuestion3 = a.Question.BasicQuestion3, BasicAnswerKey1 = a.Question.BasicAnswerKey1, BasicAnswerKey2 = a.Question.BasicAnswerKey2, BasicAnswerKey3 = a.Question.BasicAnswerKey3, InteractiveReadingAnswer = a.Question.InteractiveReadingAnswer, InteractiveReadingOptionsAnswerKey = a.Question.InteractiveReadingOptionsAnswerKey, QuestionText = a.Question.QuestionText, SentenceRepetionText = a.Question.SentenceRepetionText, RubricRaterScaleName = a.Question.RubricRaterScaleName, TranscriptLink = a.Question.TranscriptLink } }).First(a => a.Id == answerId);
                var raterAnswer = _context.RaterAnswers.FirstOrDefault(ra => ra.AnswerId == answerId && ra.RaterTestId == raterId);
                if (raterAnswer != null) {
                    RaterNotes = raterAnswer.Notes;
                    Rating = raterAnswer.Score;
                    IsDisqualified = raterAnswer.IsDisqualified;
                    IsFlagged = raterAnswer.IsFlagged;
                    IsSuspicious = raterAnswer.IsSuspicious;
                    scoreText = raterAnswer.ScoreText;
                }
                if (isFinalScorer) {
                    var otherTestIds = _context.RaterTests.Where(rt => rt.TestUserId == id && !rt.IsFinalScorer).Select(rt => rt.Id).ToList();
                    OtherAnswerList = _context.RaterAnswers.Where(ra => ra.AnswerId == answerId && otherTestIds.Contains(ra.RaterTestId)).ToList();
                    var i = 1;
                    foreach (var otherAnswer in OtherAnswerList) {
                        otherAnswer.Id = i++;
                    }
                } else {
                    OtherAnswerList = new List<RaterAnswer>();
                }
            } else if (_context.RaterAnswers != null && _context.RaterTests != null && isFinalScorer) {
                var otherRaters = _context.RaterTests.Where(rt => rt.TestUserId == id && !rt.IsFinalScorer).Select(rt => rt.Id).ToList();
                OtherAnswerList = _context.RaterAnswers.Where(ra => otherRaters.Contains(ra.RaterTestId)).ToList();
                OtherAnswerListFinal = _context.RaterAnswers.Where(ra => ra.RaterTestId == raterId).ToList();
                var assignedRaterInformation = await _context.RaterTests.Include(rt => rt.Rater).Where(rt => rt.TestUserId == id).Select(rt => new { rt.Id, rt.Rater.Email, rt.IsExtraScorer, rt.IsFinalScorer, rt.FinalScore, rt.DateFinished }).ToListAsync();
                AssignedRaters = assignedRaterInformation.Select(rt => new Tuple<string, string, string>(rt.Email, rt.IsExtraScorer ? " (Second Pass)" : rt.IsFinalScorer ? " (Final)" : "", rt.FinalScore == 0 ? "Not Scored" : "Final Score: " + rt.FinalScore.ToString("0.00"))).OrderBy(s => s.Item1).ToList();

            } else {
                OtherAnswerList = new List<RaterAnswer>();
            }

            var testId = _context.TestUsers.First(tu => tu.Id == id).TestId;
            var rubricRaterScaleName = !string.IsNullOrWhiteSpace(Answer?.Question?.RubricRaterScaleName) ? Answer?.Question?.RubricRaterScaleName : _context.Tests.First(t => t.Id == testId).RubricRaterScaleName;
            var rubricRatings = _context.RaterScales.Where(rs => rs.RaterScaleName == rubricRaterScaleName).ToList();
            if (rubricRatings.Count > 0) {
                RubricThinQuestions = RubricThinQuestion.GenerateFromDatabase(rubricRatings, scoreText);
            } else {
                RubricThinQuestions = new List<RubricThinQuestion>();
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (_context != null && _context.RaterAnswers != null) {
                Id = Request.Form["id"];
                RaterId = Request.Form["raterid"];
                var raterId = int.Parse(RaterId);
                // answering a question with a custom rubric
                if (Request.Form.ContainsKey("raterScale_0")) {
                    AnswerId = Request.Form["answerid"];
                    NextAnswerId = Request.Form["nextid"];
                    var answerId = int.Parse(AnswerId);
                    var i = 0;
                    var answers = "";
                    float totalValue = 0;
                    foreach (var key in Request.Form.Keys.Where(k => k.StartsWith("raterScale_")).OrderBy(s => s)) {
                        answers += Request.Form["raterScaleTitle_" + i] + ": " + Request.Form[key] + "; ";
                        totalValue += float.Parse(Request.Form[key]) * float.Parse(Request.Form["raterScaleWeight_" + i]);
                        i++;
                    }

                    var raterAnswer = _context.RaterAnswers.FirstOrDefault(ra => ra.AnswerId == answerId && ra.RaterTestId == raterId);
                    if (raterAnswer != null) {
                        raterAnswer.Notes = Request.Form["notes"];
                        raterAnswer.Score = totalValue;
                        raterAnswer.ScoreText = answers;
                        raterAnswer.IsAnswered = true;
                        raterAnswer.IsDisqualified = Request.Form.ContainsKey("isdisqualified");
                        raterAnswer.IsSuspicious = Request.Form.ContainsKey("issuspicious");
                        raterAnswer.IsFlagged = Request.Form.ContainsKey("isflagged");
                        _context.RaterAnswers.Update(raterAnswer);
                    } else {
                        _context.RaterAnswers.Add(new RaterAnswer {
                            AnswerId = answerId,
                            DateFinished = DateTime.Now,
                            Notes = Request.Form["notes"],
                            Score = totalValue,
                            ScoreText = answers,
                            RaterTestId = raterId,
                            IsAnswered = true,
                            IsDisqualified = Request.Form.ContainsKey("isdisqualified"),
                            IsSuspicious = Request.Form.ContainsKey("issuspicious"),
                            IsFlagged = Request.Form.ContainsKey("isflagged")
                        });
                    }
                    await _context.SaveChangesAsync();

                    if (NextAnswerId == "0") {
                        return RedirectToPage("Review", new { id = Id, raterid = RaterId, final = "true" });
                    }
                    return RedirectToPage("Review", new { id = Id, raterid = RaterId, answerId = NextAnswerId });
                } else if (Request.Form.ContainsKey("answerid")) {  // answering a single question
                    AnswerId = Request.Form["answerid"];
                    NextAnswerId = Request.Form["nextid"];

                    if (Request.Form.ContainsKey("level") || Request.Form.ContainsKey("isdisqualified")) {
                        var answerId = int.Parse(AnswerId);

                        var raterAnswer = _context.RaterAnswers.FirstOrDefault(ra => ra.AnswerId == answerId && ra.RaterTestId == raterId);
                        if (raterAnswer != null) {
                            raterAnswer.Notes = Request.Form["notes"];
                            raterAnswer.Score = Request.Form.ContainsKey("level") ? int.Parse(Request.Form["level"]) : 0;
                            raterAnswer.IsAnswered = true;
                            raterAnswer.IsDisqualified = Request.Form.ContainsKey("isdisqualified");
                            raterAnswer.IsSuspicious = Request.Form.ContainsKey("issuspicious");
                            raterAnswer.IsFlagged = Request.Form.ContainsKey("isflagged");
                            _context.RaterAnswers.Update(raterAnswer);
                        } else {
                            _context.RaterAnswers.Add(new RaterAnswer {
                                AnswerId = answerId,
                                DateFinished = DateTime.Now,
                                Notes = Request.Form["notes"],
                                Score = Request.Form.ContainsKey("level") ? int.Parse(Request.Form["level"]) : 0,
                                RaterTestId = raterId,
                                IsAnswered = true,
                                IsDisqualified = Request.Form.ContainsKey("isdisqualified"),
                                IsSuspicious = Request.Form.ContainsKey("issuspicious"),
                                IsFlagged = Request.Form.ContainsKey("isflagged")
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
                    var raterTotalScore = _context.RaterAnswers.Where(ra => ra.RaterTestId == raterId && ra.IsAnswered && !ra.IsDisqualified).Sum(ra => ra.Score);
                    var raterTotalAnswers = _context.RaterAnswers.Count(ra => ra.RaterTestId == raterId && ra.IsAnswered && !ra.IsDisqualified);
                    rater.FinalScore = raterTotalScore / raterTotalAnswers;
                    rater.Notes = Request.Form["notes"];
                    var id = int.Parse(Id);
                    var test = _context.TestUsers.Single(tu => tu.Id == id);
                    test.NumberReviewerScores++;
                    var raterName = _context.RaterNames.Single(rn => rn.Id == rater.RaterNameId);
                    raterName.NumberOfTests--;
                    await _context.SaveChangesAsync();
                    var secondaryRater = _context.RaterTests.First(rt => rt.Id == raterId).RaterAnswerRemoveIdString;

                    _finalizing.FinalizeForRater(rater.TestUserId, raterId, User.Identity?.Name ?? "", secondaryRater);
                }
            }
            return RedirectToPage("Index");
        }

        public List<RaterAnswer> OtherAnswerListById(List<RaterAnswer> list, int id) => list.Where(oal => oal.AnswerId == id).OrderBy(oal => oal.Id).ToList();

        public string UrlInfo(int answerid, int nextid) => answerid == 0 && nextid == 0 ? UrlString + "&final=true" : UrlString + "&answerid=" + answerid + "&nextid=" + nextid;
    }
}