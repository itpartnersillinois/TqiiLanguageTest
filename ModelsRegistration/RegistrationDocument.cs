using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public class RegistrationDocument {
        public string Description { get; set; } = string.Empty;

        public byte[] Document { get; set; } = Array.Empty<byte>();

        public string DocumentType { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual RegistrationTestPerson? RegistrationTestPerson { get; set; }
        public int RegistrationTestPersonId { get; set; }
        public TestType TestName { get; set; }
    }
}