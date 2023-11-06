using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class TestUserHandler {
        private readonly LanguageDbContext _context;

        public TestUserHandler(LanguageDbContext context) {
            _context = context;
        }

        public async Task<int> AddTestUser(TestUser user) {
            var existingUser = _context.TestUsers?.Where(tu => tu.UserIdentification != null && tu.UserIdentification != "").FirstOrDefault(tu => tu.Email == user.Email);
            if (existingUser != null && !string.IsNullOrEmpty(existingUser.UserIdentification)) {
                user.UserIdentification = existingUser.UserIdentification;
            } else {
                var newUser = _context.TestUsers?.Where(tu => tu.UserIdentification != null && tu.UserIdentification != "").OrderByDescending(tu => tu.UserIdentification).FirstOrDefault();
                user.UserIdentification = string.IsNullOrWhiteSpace(newUser?.UserIdentification) ? "0000000001" : (int.Parse(newUser.UserIdentification) + 1).ToString("0000000000");
            }
            var test = _context.Tests?.Find(user.TestId) ?? new Test();
            user.TotalQuestions = test.NumberQuestions;
            test.Guid = Guid.NewGuid();
            _context?.TestUsers?.Add(user);
            return await _context.SaveChangesAsync();
        }

        public TestUser? GetTestUser(Guid guid) => _context.TestUsers?.SingleOrDefault(tu => tu.Guid == guid);

        public Guid? GetTestUserGuid(string email) {
            var returnValue = _context.TestUsers?.Include(tu => tu.Test)?.Where(tu => tu.Test != null && !tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null && tu.DateTimeStart != null).OrderBy(tu => tu.OrderBy).FirstOrDefault();
            returnValue ??= _context.TestUsers?.Include(tu => tu.Test).Where(tu => tu.Test != null && !tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null).OrderBy(tu => tu.OrderBy).FirstOrDefault();
            return returnValue?.Guid;
        }
    }
}