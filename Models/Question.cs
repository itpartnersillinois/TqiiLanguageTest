using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class Question {
        public string AnswerOptions { get; set; } = string.Empty;
        public int DurationAnswerInSeconds { get; set; } = 10;
        public int DurationRecordingInSeconds { get; set; } = 60;
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderBy { get; set; }
        public virtual QuestionRubric? QuestionRubric { get; set; }
        public string QuestionText { get; set; } = string.Empty;

        public byte[] Recording { get; set; } = Array.Empty<byte>();
        public string RecordingText { get; set; } = string.Empty;
        public Test? Test { get; set; }
        public int TestId { get; set; }
        public string Title { get; set; } = string.Empty;

        [NotMapped]
        public int TotalQuestions { get; set; }
    }
}