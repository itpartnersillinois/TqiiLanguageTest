using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.BusinessLogic {

    public class RegistrationPersonHelper {
        private readonly RegistrationDbContext _context;

        public RegistrationPersonHelper(RegistrationDbContext context) {
            _context = context;
        }

        public async Task<int> AssignDocumentToTest(int id, byte[] document, string fileName, TestType testType) {
            var item = new RegistrationDocument {
                RegistrationTestPersonId = id,
                Document = document,
                FileName = fileName,
                Description = "",
                DocumentType = "",
                TestName = testType
            };
            _ = _context.Add(item);
            _ = await _context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<int> AssignPersonToCohort(int cohortId, int personId) {
            var existingItem = _context.CohortPeople?.SingleOrDefault(c => c.RegistrationCohortId == cohortId && c.RegistrationPersonId == personId);
            if (existingItem != null) {
                var tests = _context.RegistrationTestPeople?.Where(t => t.RegistrationCohortPersonId == existingItem.Id);
                if (tests != null) {
                    foreach (var test in tests) {
                        _context.RegistrationTestPeople?.Remove(test);
                    }
                    _ = await _context.SaveChangesAsync();
                }
                return existingItem.Id;
            }
            var item = new RegistrationCohortPerson {
                RegistrationCohortId = cohortId,
                RegistrationPersonId = personId
            };
            _ = _context.Add(item);
            _ = await _context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<int> AssignPersonToTest(int? testId, int cohortPersonId, bool isExempt, string language) {
            var item = new RegistrationTestPerson {
                RegistrationTestId = testId,
                RegistrationCohortPersonId = cohortPersonId,
                Language = language,
                IsProficiencyExemption = isExempt,
            };
            _ = _context.Add(item);
            _ = await _context.SaveChangesAsync();
            return item.Id;
        }

        public RegistrationCohortPerson GetCohortPerson(int id) => _context.CohortPeople?.SingleOrDefault(c => c.Id == id) ?? new RegistrationCohortPerson();

        public RegistrationPerson GetPerson(string email) => _context.People?.SingleOrDefault(c => c.Email == email) ?? new RegistrationPerson();

        public RegistrationTestPerson GetTestPerson(int id) => _context.RegistrationTestPeople?.SingleOrDefault(c => c.Id == id) ?? new RegistrationTestPerson();

        public RegistrationCohort? IsPersonAssignedToCohort(int personId) => _context.CohortPeople?.Include(c => c.RegistrationCohort).SingleOrDefault(x => x.RegistrationPersonId == personId && x.IsRegistrationCompleted)?.RegistrationCohort;

        public int? IsPersonAssignedToCohortGetId(int personId) => _context.CohortPeople?.SingleOrDefault(x => x.RegistrationPersonId == personId && x.IsRegistrationCompleted)?.Id;

        public async Task<int> MarkPersonCohortAsComplete(int id) {
            var existingItem = _context.CohortPeople?.SingleOrDefault(c => c.Id == id);
            if (existingItem != null) {
                existingItem.DateUpdated = DateTime.UtcNow;
                existingItem.IsRegistrationCompleted = true;
                _context.CohortPeople?.Update(existingItem);
                _ = await _context.SaveChangesAsync();
                return existingItem.Id;
            }
            return 0;
        }

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

        public async Task<int> UpdateCohortPerson(RegistrationCohortPerson cohortPerson) {
            _context.CohortPeople?.Update(cohortPerson);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateTestPerson(RegistrationTestPerson testPerson) {
            _context.RegistrationTestPeople?.Update(testPerson);
            return await _context.SaveChangesAsync();
        }
    }
}