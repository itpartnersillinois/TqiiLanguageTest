using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class PracticeTestHandler {
        private readonly LanguageDbContext _context;

        public PracticeTestHandler(LanguageDbContext context) {
            _context = context;
        }

        public Guid? GetTestUserGuid(string email, bool createNewTest) {
            var returnValue = _context.TestUsers?.Include(tu => tu.Test)?.Where(tu => tu.Test != null && tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null && tu.DateTimeStart != null).OrderBy(tu => tu.OrderBy).FirstOrDefault();
            returnValue ??= _context.TestUsers?.Include(tu => tu.Test).Where(tu => tu.Test != null && tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null).OrderBy(tu => tu.OrderBy).FirstOrDefault();
            if (returnValue == null && createNewTest) {
                var practiceTests = _context.Tests?.Where(t => t.IsPractice).OrderBy(t => t.PracticeOrder).ToList();

                foreach (var practiceTest in practiceTests) {
                    var testUser = new TestUser { Email = email, Guid = Guid.NewGuid(), OrderBy = practiceTest.PracticeOrder, TestId = practiceTest.Id, TotalQuestions = practiceTest.NumberQuestions, UserIdentification = "" };
                    _context.Add(testUser);
                    returnValue ??= testUser;
                }
                _context.SaveChanges();
            };
            return returnValue?.Guid;
        }
    }
}