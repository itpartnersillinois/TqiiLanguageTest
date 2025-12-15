using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public enum InstructionType {
        None,
        Introduction,
        CohortIntroduction,
        Iein,
        LangaugeProficiency1,
        LangaugeProficiency2,
        SpedProficiency,
        InterpreterProficiency,
        Conclusion,
        EmailApproved,
        EmailDenied,
        Waitlisted
    }

    public class RegistrationInstruction {
        public string Description { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string InstructionText { get; set; } = string.Empty;

        public InstructionType TypeOfInstruction { get; set; }
    }
}