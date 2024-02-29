using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public enum TestEnum {
        None,
        SentenceRepetition, // Listen to a recording (introduction), optional multiple choice or reflection, record answer
        IntegratedSpeaking, // Listen to a recording (introduction), optional reflection, record answer
        InteractiveReading  // Type in text to complete information or choose from a drop-down
    }

    public class Test {
        public string Conclusion { get; set; } = string.Empty;
        public string ConclusionLink { get; set; } = string.Empty;
        public byte[] ConclusionRecording { get; set; } = Array.Empty<byte>();
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Introduction { get; set; } = string.Empty;
        public byte[] IntroductionRecording { get; set; } = Array.Empty<byte>();
        public bool IsPractice { get; set; }
        public string Language { get; set; } = string.Empty;
        public int NumberQuestions { get; set; }
        public int PracticeOrder { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
        public TestEnum TestType { get; set; }
        public string? Title { get; set; }
    }
}