using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class Answer {

        [NotMapped]
        public string AnswerOptions { get; set; } = string.Empty;

        public string BasicAnswers1 { get; set; } = string.Empty;
        public string BasicAnswers2 { get; set; } = string.Empty;
        public string BasicAnswers3 { get; set; } = string.Empty;
        public List<string> BasicOptions1 => string.IsNullOrWhiteSpace(BasicAnswers1) ? new List<string>() : BasicAnswers1.Split("|").Select(i => i.Trim()).ToList();
        public List<string> BasicOptions2 => string.IsNullOrWhiteSpace(BasicAnswers2) ? new List<string>() : BasicAnswers2.Split("|").Select(i => i.Trim()).ToList();
        public List<string> BasicOptions3 => string.IsNullOrWhiteSpace(BasicAnswers3) ? new List<string>() : BasicAnswers3.Split("|").Select(i => i.Trim()).ToList();

        [NotMapped]
        public string BasicQuestion1 { get; set; } = string.Empty;

        [NotMapped]
        public string BasicQuestion2 { get; set; } = string.Empty;

        [NotMapped]
        public string BasicQuestion3 { get; set; } = string.Empty;

        public List<string> ButtonAnswers => string.IsNullOrWhiteSpace(AnswerOptions) ? new List<string>() : AnswerOptions.Split("|").Select(i => i.Trim()).ToList();

        public List<string> ButtonInteractiveReadingOptions => string.IsNullOrWhiteSpace(InteractiveReadingOptions) ? new List<string>() : InteractiveReadingOptions.Split("|").Select(i => i.Trim()).ToList();

        public List<string> ButtonInteractiveReadingOptionsDropDown => string.IsNullOrWhiteSpace(InteractiveReadingOptionsDropDown) ? ButtonInteractiveReadingOptions : InteractiveReadingOptionsDropDown.Split("|").Select(i => i.Trim()).ToList();

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

        public bool HasInteractiveReadingImage => InteractiveReadingImage != null && InteractiveReadingImage.Length > 0;

        public bool HasQuestionImage => QuestionImage != null && QuestionImage.Length > 0;

        public bool HasRecordingImage => RecordingImage != null && RecordingImage.Length > 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [NotMapped]
        public string InteractiveReadingAnswer { get; set; } = string.Empty;

        [NotMapped]
        public byte[] InteractiveReadingImage { get; set; } = Array.Empty<byte>();

        [NotMapped]
        public string InteractiveReadingOptions { get; set; } = string.Empty;

        [NotMapped]
        public string InteractiveReadingOptionsDropDown { get; set; } = string.Empty;

        [NotMapped]
        public string LanguageCharacters { get; set; } = string.Empty;

        [NotMapped]
        public bool LanguagePopout { get; set; }

        public int NumberTimesRefreshed { get; set; } = 0;
        public int OrderBy { get; set; }
        public virtual Question? Question { get; set; }

        [NotMapped]
        public Guid QuestionGuid { get; set; }

        public int? QuestionId { get; set; }

        [NotMapped]
        public byte[] QuestionImage { get; set; } = Array.Empty<byte>();

        [NotMapped]
        public string QuestionText { get; set; } = string.Empty;

        [NotMapped]
        public QuestionEnum QuestionType { get; set; }

        public byte[] Recording { get; set; } = Array.Empty<byte>();

        [NotMapped]
        public byte[] RecordingImage { get; set; } = Array.Empty<byte>();

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