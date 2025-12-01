using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public class RegistrationPerson {
        public string Country { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Iein { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NativeLanguage { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}