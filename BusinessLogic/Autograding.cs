using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class Autograding {
        private readonly LanguageDbContext _context;

        public Autograding(LanguageDbContext context) {
            _context = context;
        }

        public void AutoGrade(DateTime currentDate, TestUser testUser, int testUserId) {
            var autogradedQuestions = _context.Questions.Where(q => q.TestId == testUser.TestId && q.InteractiveReadingOptionsAnswerKey != "").Select(q => new { q.Id, q.InteractiveReadingOptionsAnswerKey }).ToList();
            var autogradedQuestionsBasedOnQuestions = _context.Questions.Where(q => q.TestId == testUser.TestId && q.BasicAnswerKey1 != "").Select(q => new { q.Id, q.BasicAnswerKey1, q.BasicAnswerKey2, q.BasicAnswerKey3 }).ToList();
            if ((autogradedQuestions.Any() || autogradedQuestionsBasedOnQuestions.Any())) {
                var autograders = _context.RaterTests.Where(rt => rt.TestUserId == testUserId && rt.DateAssigned == currentDate);
                var answers = _context.Answers.Where(a => a.TestUserId == testUserId).Select(a => new { a.QuestionId, a.Id, a.Text, a.BasicAnswers1, a.BasicAnswers2, a.BasicAnswers3 }).ToList();
                foreach (var autogradeQuestion in autogradedQuestions) {
                    var answer = answers.SingleOrDefault(a => (a.QuestionId ?? 0) == autogradeQuestion.Id);
                    if (answer != null) {
                        var score = 0;
                        var answerArray = PullAnswers(answer.Text);
                        var answerKeyArray = PullAnswers(autogradeQuestion.InteractiveReadingOptionsAnswerKey);
                        for (var i = 0; i < answerKeyArray.Length; i++) {
                            if (i < answerArray.Length && answerArray[i] == answerKeyArray[i]) {
                                score++;
                            }
                        }
                        var total = score * 100 / answerKeyArray.Length;
                        foreach (var autograder in autograders) {
                            _context.RaterAnswers.Add(new RaterAnswer {
                                Score = total,
                                AnswerId = answer.Id,
                                DateFinished = currentDate,
                                Notes = $"autograded with {score} / {answerKeyArray.Length}",
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
                        total = answer != null && answer.BasicAnswers1.Trim() == autogradeQuestion.BasicAnswerKey1.Trim() ? total + 1 : total;
                    }
                    if (autogradeQuestion.BasicAnswerKey2 != "") {
                        count++;
                        total = answer != null && answer.BasicAnswers2.Trim() == autogradeQuestion.BasicAnswerKey2.Trim() ? total + 1 : total;
                    }
                    if (autogradeQuestion.BasicAnswerKey3 != "") {
                        count++;
                        total = answer != null && answer.BasicAnswers3.Trim() == autogradeQuestion.BasicAnswerKey3.Trim() ? total + 1 : total;
                    }

                    if (answer != null) {
                        foreach (var autograder in autograders) {
                            _context.RaterAnswers.Add(new RaterAnswer {
                                Score = total * 100 / count,
                                AnswerId = answer.Id,
                                DateFinished = currentDate,
                                Notes = $"autograded with {total} / {count}",
                                RaterTestId = autograder.Id,
                            });
                        }
                    }
                }
                _context.SaveChanges();
            }
        }

        private string[] PullAnswers(string s) => s.Split('[').Where(item => item.Contains(']')).Select(item => item.Substring(0, item.IndexOf("]"))).ToArray();
    }
}