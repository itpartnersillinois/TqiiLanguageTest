using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class Finalizing {
        private readonly LanguageDbContext _context;

        public Finalizing(LanguageDbContext context) {
            _context = context;
        }

        public void FinalizeForRater(int testUserId, int raterTestId, string raterEmail) {
            var testUser = _context.TestUsers.Include(tu => tu.Test).Single(tu => tu.Id == testUserId);
            var raterAnswers = _context.RaterAnswers.Where(ra => ra.RaterTestId == raterTestId).ToList();
            var rater = _context.RaterNames.FirstOrDefault(ra => ra.Email == raterEmail);
            var raterName = rater == null || string.IsNullOrWhiteSpace(rater.FullName) ? raterEmail : rater.FullName;
            var answers = _context.Answers.Include(a => a.Question).Where(a => a.TestUserId == testUserId && a.Question.QuestionType != QuestionEnum.Instructions).Select(a => new { a.Id, a.QuestionId, a.Question.Title, a.DateTimeEnd, a.Question.QuestionType }).ToList();

            foreach (var answer in answers) {
                var raterAnswer = raterAnswers.FirstOrDefault(ra => ra.AnswerId == answer.Id);
                // if raterAnswer has an autograde response, then split it out into individual items
                if (raterAnswer != null && !string.IsNullOrWhiteSpace(raterAnswer.AutogradedNotes)) {
                    var specificAnswers = raterAnswer.AutogradedNotes.Split(";");
                    foreach (var specificAnswer in specificAnswers) {
                        var answerArray = specificAnswer.Split(",");
                        _context.ReportDetails.Add(new ReportDetail {
                            Email = testUser.Email,
                            UserIdentification = testUser.UserIdentification ?? "",
                            TestUserId = testUser.Id,
                            QuestionId = answer.QuestionId ?? 0,
                            AnswerId = answer.Id,
                            TestDate = testUser.DateTimeEnd ?? DateTime.Now,
                            TestName = testUser.Test?.Title ?? "",
                            QuestionName = answer.Title,
                            QuestionType = answer.QuestionType.ToString(),
                            QuestionAnswered = answer.DateTimeEnd ?? DateTime.Now,
                            Answer = answerArray[0],
                            AnswerKey = answerArray[1],
                            AutogradedScore = answerArray[2],
                            RaterName = raterName,
                            RaterScore = raterAnswer?.Score ?? 0,
                            RaterNotes = raterAnswer?.Notes ?? ""
                        });
                    }
                } else {
                    _context.ReportDetails.Add(new ReportDetail {
                        Email = testUser.Email,
                        UserIdentification = testUser.UserIdentification ?? "",
                        TestUserId = testUser.Id,
                        QuestionId = answer.QuestionId ?? 0,
                        AnswerId = answer.Id,
                        TestDate = testUser.DateTimeEnd ?? DateTime.Now,
                        TestName = testUser.Test?.Title ?? "",
                        QuestionName = answer.Title,
                        QuestionType = answer.QuestionType.ToString(),
                        QuestionAnswered = answer.DateTimeEnd ?? DateTime.Now,
                        Answer = "",
                        AnswerKey = "",
                        AutogradedScore = "",
                        RaterName = raterName,
                        RaterScore = raterAnswer?.Score ?? 0,
                        RaterNotes = raterAnswer?.Notes ?? ""
                    });
                }
            }
            _context.SaveChanges();
        }

        public void FinalizeForTest(TestUser testUser) {
            var test = _context.Tests.FirstOrDefault(t => t.Id == testUser.TestId);
            var isbeReportRow = _context.ReportIsbes.FirstOrDefault(r => r.Email == testUser.Email && r.UserIdentification == testUser.UserIdentification);
            if (isbeReportRow != null) {
                if (test.TestType == TestEnum.SentenceRepetition) {
                    isbeReportRow.SentenceRepetitionScore = testUser.Score;
                } else if (test.TestType == TestEnum.InteractiveReading) {
                    isbeReportRow.InteractiveReadingScore = testUser.Score;
                } else if (test.TestType == TestEnum.IntegratedSpeaking) {
                    isbeReportRow.IntegratedSpeakingScore = testUser.Score;
                }
                isbeReportRow.TestDate = testUser.DateTimeEnd ?? DateTime.Now;
                isbeReportRow.TestName = test.Title ?? "";
                if (isbeReportRow.SentenceRepetitionScore > 0 && isbeReportRow.InteractiveReadingScore > 0 && isbeReportRow.IntegratedSpeakingScore > 0) {
                    isbeReportRow.TotalScore = (isbeReportRow.SentenceRepetitionScore + isbeReportRow.InteractiveReadingScore + isbeReportRow.IntegratedSpeakingScore) / 3;
                }
            } else {
                _context.ReportIsbes.Add(new ReportIsbe {
                    Email = testUser.Email,
                    UserIdentification = testUser.UserIdentification,
                    IntegratedSpeakingScore = test.TestType == TestEnum.IntegratedSpeaking ? testUser.Score : 0,
                    InteractiveReadingScore = test.TestType == TestEnum.InteractiveReading ? testUser.Score : 0,
                    SentenceRepetitionScore = test.TestType == TestEnum.SentenceRepetition ? testUser.Score : 0,
                    TestDate = testUser.DateTimeEnd ?? DateTime.Now,
                    TestName = test.Title ?? ""
                });
            }

            var detailReportRows = _context.ReportDetails.Where(r => r.TestUserId == testUser.Id).ToList();
            foreach (var detail in detailReportRows) {
                detail.TotalScore = testUser.Score;
            }
            _context.SaveChanges();
        }
    }
}