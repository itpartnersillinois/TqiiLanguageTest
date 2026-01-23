using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.BusinessLogic {

    public class RegistrationTestHelper {
        private readonly RegistrationDbContext _context;
        private readonly LanguageDbContext _languageContext;

        public RegistrationTestHelper(RegistrationDbContext context, LanguageDbContext languageContext) {
            _context = context;
            _languageContext = languageContext;
        }

        public RegistrationCohort GetCohort(int id) => _context.Cohorts?.SingleOrDefault(c => c.Id == id) ?? new RegistrationCohort();

        public List<RegistrationCohortPerson> GetCohortPeople(int cohortId) => _context.CohortPeople?.Include(r => r.RegistrationPerson).Where(r => r.RegistrationCohortId == cohortId && r.IsRegistrationCompleted && r.RegistrationPerson != null).OrderBy(r => r.RegistrationPerson.LastName).ThenBy(r => r.RegistrationPerson.FirstName).ToList() ?? new List<RegistrationCohortPerson>();

        public List<RegistrationCohortPerson> GetCohortPeopleIncomplete(int cohortId) => _context.CohortPeople?.Include(r => r.RegistrationPerson).Where(r => r.RegistrationCohortId == cohortId && !r.IsRegistrationCompleted && r.RegistrationPerson != null).OrderBy(r => r.RegistrationPerson.LastName).ThenBy(r => r.RegistrationPerson.FirstName).ToList() ?? new List<RegistrationCohortPerson>();

        public List<RegistrationCohort> GetCohorts() {
            var returnValue = _context.Cohorts?.OrderBy(c => c.StartDate).ToList() ?? new List<RegistrationCohort>();
            foreach (var cohort in returnValue) {
                cohort.NumberStudentsApplied = _context.CohortPeople?.Count(cp => cp.RegistrationCohortId == cohort.Id) ?? 0;
                cohort.NumberStudentsEnrolled = _context.CohortPeople?.Count(cp => cp.RegistrationCohortId == cohort.Id && (cp.DateRegistered != null || cp.IsApproved)) ?? 0;
            }
            return returnValue;
        }

        public List<string> GetLanguages() => _languageContext.LanguageOptions?.OrderBy(r => r.Language).Select(r => r.Language).ToList() ?? new List<string>();

        public RegistrationTest GetTest(int id) => _context.RegistrationTests?.SingleOrDefault(r => r.Id == id) ?? new RegistrationTest();

        public List<RegistrationTestPerson> GetTestPeople(int cohortId) => _context.RegistrationTestPeople?.Include(rt => rt.RegistrationDocument).Include(rt => rt.RegistrationTest).Include(rt => rt.RegistrationCohortPerson).Where(r => r.RegistrationCohortPerson != null && r.RegistrationCohortPerson.RegistrationCohortId == cohortId).ToList() ?? new List<RegistrationTestPerson>();

        public List<RegistrationTest> GetTests(int cohortId) => _context.RegistrationTests?.Where(r => r.RegistrationCohortId == cohortId).OrderBy(r => r.TestName).ToList() ?? new List<RegistrationTest>();

        public async Task<int> SaveCohort(RegistrationCohort cohort) {
            cohort.Description ??= "";
            cohort.TestName ??= "";
            if (cohort.Id == 0) {
                _ = _context.Add(cohort);
            } else if (cohort.TestName == "") {
                _context.Cohorts?.Remove(cohort);
            } else {
                _context.Cohorts?.Update(cohort);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveTest(RegistrationTest test) {
            test.Description ??= "";
            test.TestName ??= "";
            test.RegistrationLink ??= "";
            test.Language ??= "";
            if (test.Id == 0) {
                _ = _context.Add(test);
            } else if (test.TestName == "") {
                _context.RegistrationTests?.Remove(test);
            } else {
                _context.RegistrationTests?.Update(test);
            }
            return await _context.SaveChangesAsync();
        }
    }
}