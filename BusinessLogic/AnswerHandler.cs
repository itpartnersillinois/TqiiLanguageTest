using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class AnswerHandler {
        private readonly LanguageDbContext _context;
        private readonly TestUserHandler _testUserHandler;

        public AnswerHandler(LanguageDbContext context, TestUserHandler testUserHandler) {
            _context = context;
            _testUserHandler = testUserHandler;
        }

        public async Task<Answer?> GetAnswer(Guid testUser) {
            var testUserObject = _testUserHandler.GetTestUser(testUser);
            if (testUserObject == null) {
                return null;
            }

            var question = _context.Questions?.SingleOrDefault(q => q.Id == testUserObject.CurrentQuestionId);
            if (question == null) {
                return null;
            }
            var answer = _context.Answers?.SingleOrDefault(q => q.TestUserId == testUserObject.Id && q.QuestionId == question.Id);
            if (answer == null) {
                answer = new Answer {
                    ReviewerNotes = "Starting",
                    Question = question,
                    TestUser = testUserObject
                };
                _context.Add(answer);
            } else if (answer.ReviewerNotes == "Starting") {
                return null;
            } else {
                answer.ReviewerNotes = "Starting";
            }
            _ = await _context.SaveChangesAsync();
            answer.CurrentQuestionNumber = testUserObject.CurrentQuestionOrder;
            answer.TotalQuestions = testUserObject.TotalQuestions;
            answer.AnswerOptions = question.AnswerOptions;
            answer.QuestionText = question.QuestionText;
            answer.RecordingText = question.RecordingText;
            answer.DurationAnswerInSeconds = question.DurationAnswerInSeconds;
            answer.DurationRecordingInSeconds = question.DurationRecordingInSeconds;
            return answer;
        }

        public async Task<string> SetRecording(Guid answerGuid, byte[] recording) {
            var answer = _context.Answers?.SingleOrDefault(a => a.Guid == answerGuid);
            if (answer == null) {
                return "not found";
            }
            if (answer.Recording != null && answer.Recording.Length > 0) {
                return "already answered";
            }
            answer.Recording = recording;
            answer.DateTimeEnd = DateTime.Now;
            answer.ReviewerNotes = "";
            _ = await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> SetText(Guid answerGuid, string answerText) {
            var answer = _context.Answers?.SingleOrDefault(a => a.Guid == answerGuid);
            if (answer == null) {
                return "not found";
            }
            if (!string.IsNullOrWhiteSpace(answer.Text)) {
                return "already answered";
            }
            answer.Text = answerText;
            answer.DateTimeTextAnswered = DateTime.Now;
            answer.ReviewerNotes = "";
            _ = await _context.SaveChangesAsync();
            return string.Empty;
        }
    }
}