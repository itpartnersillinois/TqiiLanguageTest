using TqiiLanguageTest.Data;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.BusinessLogic {

    public class RegistrationPersonHelper {
        private readonly RegistrationDbContext _context;

        public RegistrationPersonHelper(RegistrationDbContext context) {
            _context = context;
        }

        public async Task<int> AssignDocumentToTest(int id, byte[] document, string fileName) {
            var item = new RegistrationDocument {
                RegistrationTestPersonId = id,
                Document = document,
                FileName = fileName,
                Description = "",
                DocumentType = "",
                TestName = TestType.ProficiencyExam1
            };
            _ = _context.Add(item);
            _ = await _context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<int> AssignPersonToCohort(int cohortId, int personId) {
            var item = new RegistrationCohortPerson {
                RegistrationCohortId = cohortId,
                RegistrationPersonId = personId
            };
            _ = _context.Add(item);
            _ = await _context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<int> AssignPersonToTest(int testId, int cohortPersonId) {
            var item = new RegistrationTestPerson {
                RegistrationTestId = testId,
                RegistrationCohortPersonId = cohortPersonId
            };
            _ = _context.Add(item);
            _ = await _context.SaveChangesAsync();
            return item.Id;
        }

        public RegistrationPerson GetPerson(string email) => _context.People?.SingleOrDefault(c => c.Email == email) ?? new RegistrationPerson();

        public async Task<int> SavePerson(RegistrationPerson person, string email) {
            person.Email = email;
            person.FirstName ??= "";
            person.Country ??= "";
            person.NativeLanguage ??= "";
            person.LastName ??= "";
            person.State ??= "";
            person.Iein ??= "";
            if (person.Id == 0) {
                _ = _context.Add(person);
            } else {
                _context.People?.Update(person);
            }
            return await _context.SaveChangesAsync();
        }
    }
}