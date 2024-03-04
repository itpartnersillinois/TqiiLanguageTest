using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public enum ImageTypeEnum {
        IntroductionRecording,
        Introduction,
        Question,
        Recording,
        InteractiveReading
    }

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
            var currentAnswer = _context?.Answers.Include(a => a.Question).FirstOrDefault(a => a.TestUserId == testUserObject.Id && a.DateTimeEnd == null);
            if (currentAnswer != null) {
                currentAnswer.NumberTimesRefreshed++;
                _ = await _context.SaveChangesAsync();
                return currentAnswer.Question;
            }
            testUserObject.CurrentQuestionOrder++;
            returnValue = GetQuestionByTestAndOrder(testUserObject.TestId, testUserObject.Id, testUserObject.CurrentQuestionOrder);
            if (returnValue != null) {
                testUserObject.CurrentQuestionId = returnValue.Id;
                returnValue.CurrentQuestionNumber = testUserObject.CurrentQuestionOrder;
                returnValue.TotalQuestions = testUserObject.TotalQuestions;
            } else {
                testUserObject.DateTimeEnd = DateTime.Now;
            }
            if (testUserObject.DateTimeStart == null) {
                testUserObject.DateTimeStart = DateTime.Now;
            }
            _ = await _context.SaveChangesAsync();
            return returnValue;
        }

        public async Task<string> SaveByteArray(int id, byte[] image, ImageTypeEnum imageType) {
            var question = _context.Questions?.SingleOrDefault(q => q.Id == id);
            if (question == null) {
                return "question not found";
            }
            switch (imageType) {
                case ImageTypeEnum.IntroductionRecording:
                    question.Recording = image;
                    break;

                case ImageTypeEnum.Introduction:
                    question.IntroductionImage = image;
                    break;

                case ImageTypeEnum.Question:
                    question.QuestionImage = image;
                    break;

                case ImageTypeEnum.Recording:
                    question.RecordingImage = image;
                    break;

                case ImageTypeEnum.InteractiveReading:
                    question.InteractiveReadingImage = image;
                    break;
            }
            _ = await _context.SaveChangesAsync();
            return string.Empty;
        }

        internal Question? GetQuestionByTestAndOrder(int id, int testUserId, int orderBy) {
            if (orderBy < 0) {
                orderBy = 0;
            }
            var questionsAlreadyAsked = _context?.Answers?.Where(a => a.TestUserId == testUserId).Select(a => a.QuestionId).ToList() ?? new List<int?>();
            return _context?.Questions?
                .Where(q => q.TestId == id && q.OrderBy >= orderBy && !questionsAlreadyAsked.Contains(q.Id))
                .OrderBy(q => q.OrderBy).ThenBy(q => Guid.NewGuid()).FirstOrDefault();
        }
    }
}