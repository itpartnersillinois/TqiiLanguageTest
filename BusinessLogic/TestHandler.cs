using System.Text;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class TestHandler {
        private readonly LanguageDbContext _context;
        private readonly TestUserHandler _testUserHandler;

        public TestHandler(LanguageDbContext context, TestUserHandler testUserHandler) {
            _context = context;
            _testUserHandler = testUserHandler;
        }

        public Test? GetTest(Guid testUser) {
            var testUserObject = _testUserHandler.GetTestUser(testUser);
            if (testUserObject == null) {
                return null;
            }
            var returnValue = _context.Tests?.Single(t => t.Id == testUserObject.TestId);
            returnValue.Introduction = ConvertToHtml(returnValue.Introduction);
            returnValue.Conclusion = ConvertToHtml(returnValue.Conclusion);
            return returnValue;
        }

        public async Task<string> SaveByteArray(int id, byte[] audio, int imageType) {
            var test = _context.Tests?.SingleOrDefault(q => q.Id == id);
            if (test == null) {
                return "test not found";
            }
            switch (imageType) {
                case 0:
                    test.IntroductionRecording = audio;
                    break;

                case 1:
                    test.ConclusionRecording = audio;
                    break;
            }
            _ = await _context.SaveChangesAsync();
            return string.Empty;
        }

        internal static string ConvertToHtml(string s) {
            var returnValue = new StringBuilder();
            foreach (var c in s) {
                if (c == '\r') {
                    returnValue.Append("<br>");
                } else if (c != '\n') {
                    returnValue.Append(c);
                }
            }
            return returnValue.ToString();
        }
    }
}