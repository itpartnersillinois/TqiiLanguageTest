using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class QuestionHandler {
        private readonly LanguageDbContext _context;
        private readonly TestUserHandler _testUserHandler;

        public QuestionHandler(LanguageDbContext context, TestUserHandler testUserHandler) {
            _context = context;
            _testUserHandler = testUserHandler;
        }

        public async Task<Question?> GetQuestion(Guid testUser) {
            // get test user, determine if there are more questions. If so, then assign next question and update test user. If not, then end test and return null.

            Question? returnValue = null;
            var testUserObject = _testUserHandler.GetTestUser(testUser);
            if (testUserObject == null) {
                return returnValue;
            }
            if (testUserObject.DateTimeStart == null) {
                testUserObject.DateTimeStart = DateTime.Now;
            }
            if (testUserObject.CurrentQuestionOrder == testUserObject.TotalQuestions) {
                testUserObject.DateTimeEnd = DateTime.Now;
            } else {
                testUserObject.CurrentQuestionOrder++;
                returnValue = GetQuestionByTestAndOrder(testUserObject.TestId, testUserObject.CurrentQuestionOrder);
                if (returnValue != null) {
                    testUserObject.CurrentQuestionId = returnValue.Id;
                    returnValue.TotalQuestions = testUserObject.TotalQuestions;
                }
            }
            _ = await _context.SaveChangesAsync();
            return returnValue;
        }

        public async Task<string> SaveRecording(int id, byte[] recording) {
            var question = _context.Questions?.SingleOrDefault(q => q.Id == id);
            if (question == null) {
                return "question not found";
            }
            question.Recording = recording;
            _ = await _context.SaveChangesAsync();
            return string.Empty;
        }

        internal Question? GetQuestionByTestAndOrder(int id, int orderBy) => _context?.Questions?
            .Where(q => q.TestId == id && q.OrderBy <= orderBy)
            .OrderBy(q => q.OrderBy).ThenBy(q => Guid.NewGuid()).FirstOrDefault();
    }
}