using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.BusinessLogic {

    public class TestUserHandler {
        private readonly LanguageDbContext _context;

        public TestUserHandler(LanguageDbContext context) {
            _context = context;
        }

        public TestUser? GetTestUser(Guid guid) => _context.TestUsers?.SingleOrDefault(tu => tu.Guid == guid);

        public Guid? GetTestUserGuid(string email) => _context.TestUsers?.Where(tu => tu.Email == email).OrderBy(tu => tu.OrderBy).FirstOrDefault()?.Guid;
    }
}