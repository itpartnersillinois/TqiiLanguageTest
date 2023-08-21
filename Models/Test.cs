using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

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
        public int NumberQuestions { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
        public string? Title { get; set; }
    }
}