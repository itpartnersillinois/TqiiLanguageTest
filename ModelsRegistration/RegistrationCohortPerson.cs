using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public class RegistrationCohortPerson {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateRegistered { get; set; }
        public DateTime? DateRegistrationSent { get; set; }
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public string ExternalComment { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string InternalComment { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsDenied { get; set; }
        public bool IsRegistered => DateRegistered.HasValue;
        public bool IsWaitlisted { get; set; }
        public string Language { get; set; } = string.Empty;
        public virtual RegistrationCohort? RegistrationCohort { get; set; }
        public int RegistrationCohortId { get; set; }
        public Guid RegistrationGuid { get; set; } = Guid.NewGuid();
        public virtual RegistrationPerson? RegistrationPerson { get; set; }
        public int RegistrationPersonId { get; set; }
    }
}