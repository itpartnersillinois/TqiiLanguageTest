using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.BusinessLogic {

    public class PermissionsHandler {
        private readonly LanguageDbContext _context;

        public PermissionsHandler(LanguageDbContext context) {
            _context = context;
        }

        public bool IsAdmin(string email) => _context.Permissions?.SingleOrDefault(p => p.Email == email)?.IsAdministrator ?? false;

        public bool IsItemWriter(string email) => _context.Permissions?.SingleOrDefault(p => p.Email == email)?.IsItemWriter ?? false;

        public bool IsRegistrationReviewer(string email) => _context.Permissions?.SingleOrDefault(p => p.Email == email)?.IsRegistrationReviewer ?? false;

        public bool IsReviewer(string email) => _context.Permissions?.SingleOrDefault(p => p.Email == email)?.IsReviewer ?? false;
    }
}