using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public enum TestType {
        None,
        ProficiencyExam1,
        ProficiencyExam2,
        SpecialEducation,
        Interpreter,
        Other = 9
    }

    public class RegistrationTest {
        public string DateString => $"{StartDate:d}-{EndDate:d}";
        public string Description { get; set; } = string.Empty;

        public DateTime EndDate { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Language { get; set; } = string.Empty;
        public virtual RegistrationCohort? RegistrationCohort { get; set; }
        public int RegistrationCohortId { get; set; }
        public string RegistrationLink { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string TestName { get; set; } = string.Empty;
        public TestType TypeOfTest { get; set; }
    }
}