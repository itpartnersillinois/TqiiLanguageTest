using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class AnswerHandler {
        private readonly LanguageDbContext _context;
        private readonly QuestionHandler _questionHandler;
        private readonly TestUserHandler _testUserHandler;

        public AnswerHandler(LanguageDbContext context, QuestionHandler questionHandler, TestUserHandler testUserHandler) {
            _context = context;
            _questionHandler = questionHandler;
            _testUserHandler = testUserHandler;
        }

        public async Task<Guid?> CreateAnswer(Guid testUser) {
            var testUserObject = _testUserHandler.GetTestUser(testUser);
            if (testUserObject == null) {
                return null;
            }

            var question = _context.Questions?.SingleOrDefault(q => q.Id == testUserObject.CurrentQuestionId);
            if (question == null) {
                return null;
            }
            var answer = new Answer {
                Question = question,
                TestUser = testUserObject
            };

            _context.Add(answer);
            _ = await _context.SaveChangesAsync();
            return answer.Guid;
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
            _ = await _context.SaveChangesAsync();
            return string.Empty;
        }
    }
}