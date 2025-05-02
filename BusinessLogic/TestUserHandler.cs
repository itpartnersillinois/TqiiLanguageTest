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

            var existingTestUser = _context.TestUsers?.FirstOrDefault(t => t.TestId == user.TestId && t.Email == user.Email);
            if (existingTestUser == null) {
                var test = _context.Tests?.Find(user.TestId) ?? new Test();
                user.TotalQuestions = test.NumberQuestions;
                test.Guid = Guid.NewGuid();
                _context?.TestUsers?.Add(user);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public bool DidUserCompleteAnyTests(string email) =>
            _context.TestUsers != null && _context.TestUsers.Any(tu => tu.Email == email && tu.DateTimeEnd != null);

        public TestUser? GetTestUser(Guid guid) => _context.TestUsers?.SingleOrDefault(tu => tu.Guid == guid);

        public List<Tuple<Guid?, DateTime?, DateTime?, string>> GetTestUserGuid(string email) {
            var returnValue = _context.TestUsers?.Include(tu => tu.Test)?.Where(tu => tu.Test != null && !tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null && tu.DateTimeStart != null).OrderBy(tu => tu.OrderBy).ThenByDescending(tu => tu.DateTimeScheduled);
            if (returnValue != null && returnValue.Count() > 0) {
                return returnValue.Select(r => new Tuple<Guid?, DateTime?, DateTime?, string>(r.Guid, r.DateTimeScheduled, r.DateTimeExpired, r.Language)).ToList();
            }
            var returnValuePastTests = _context.TestUsers?.Include(tu => tu.Test)?.Where(tu => tu.Test != null && !tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null).OrderBy(tu => tu.OrderBy).ThenByDescending(tu => tu.DateTimeScheduled);
            return returnValuePastTests == null ? new List<Tuple<Guid?, DateTime?, DateTime?, string>>() : returnValuePastTests.Select(r => new Tuple<Guid?, DateTime?, DateTime?, string>(r.Guid, r.DateTimeScheduled, r.DateTimeExpired, r.Language)).ToList();
        }

        public Guid? GetTestUserGuidNextTestOnly(string email) =>
            _context.TestUsers?.Include(tu => tu.Test)?.Where(tu => tu.Test != null && !tu.Test.IsPractice && tu.Email == email && tu.DateTimeEnd == null).OrderBy(tu => tu.OrderBy).FirstOrDefault()?.Guid;
    }
}