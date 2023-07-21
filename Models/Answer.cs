using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class Answer {

        [NotMapped]
        public string AnswerOptions { get; set; } = string.Empty;

        [NotMapped]
        public int CurrentQuestionNumber { get; set; }

        public DateTime? DateTimeEnd { get; set; }

        public DateTime? DateTimeStart { get; set; } = DateTime.Now;
        public DateTime? DateTimeTextAnswered { get; set; }

        [NotMapped]
        public int DurationAnswerInSeconds { get; set; } = 10;

        [NotMapped]
        public int DurationRecordingInSeconds { get; set; } = 60;

        public Guid Guid { get; set; } = Guid.NewGuid();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderBy { get; set; }
        public virtual Question? Question { get; set; }
        public int? QuestionId { get; set; }
        public virtual QuestionRubric? QuestionRubric { get; set; }

        [NotMapped]
        public string QuestionText { get; set; } = string.Empty;

        public byte[] Recording { get; set; } = Array.Empty<byte>();

        [NotMapped]
        public string RecordingText { get; set; } = string.Empty;

        public string? ReviewerNotes { get; set; }
        public string? RubricInformation { get; set; }
        public virtual TestUser? TestUser { get; set; }
        public int? TestUserId { get; set; }
        public string Text { get; set; } = "";

        [NotMapped]
        public int TotalQuestions { get; set; }
    }
}