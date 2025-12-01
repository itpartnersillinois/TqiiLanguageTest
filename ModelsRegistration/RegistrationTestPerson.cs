using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public class RegistrationTestPerson {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public string ExternalComment { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string InternalComment { get; set; } = string.Empty;
        public bool IsProficiencyExemption { get; set; }
        public bool IsProficiencyExemptionApproved { get; set; }
        public bool IsProficiencyExemptionDenied { get; set; }
        public string Language { get; set; } = string.Empty;
        public virtual RegistrationCohortPerson? RegistrationCohortPerson { get; set; }
        public int RegistrationCohortPersonId { get; set; }
        public virtual RegistrationTest? RegistrationTest { get; set; }
        public int RegistrationTestId { get; set; }
    }
}