using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class TestUserHandler {
        private readonly LanguageDbContext _context;

        public TestUserHandler(LanguageDbContext context) {
            _context = context;
        }

        public TestUser? GetTestUser(Guid guid) => _context.TestUsers?.SingleOrDefault(tu => tu.Guid == guid);

        public Guid? GetTestUserGuid(string email) {
            var returnValue = _context.TestUsers?.Where(tu => tu.Email == email && tu.DateTimeEnd == null && tu.DateTimeStart != null).OrderBy(tu => tu.OrderBy).FirstOrDefault();
            returnValue ??= _context.TestUsers?.Where(tu => tu.Email == email && tu.DateTimeEnd == null).OrderBy(tu => tu.OrderBy).FirstOrDefault();
            return returnValue?.Guid;
        }
    }
}