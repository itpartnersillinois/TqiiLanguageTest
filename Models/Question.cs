using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public enum QuestionEnum {
        None,
        SentenceRepetition, // Listen to a recording (introduction), optional multiple choice or reflection, record answer
        IntegratedSpeaking, // Listen to a recording (introduction), optional reflection, record answer
        InteractiveReading, // Type in text to complete information or choose from a drop-down
        BasicQuestions,     // Choose a answer
        Instructions        // Basic instructions
    }

    public class Question {

        private readonly Dictionary<QuestionEnum, string> Titles = new Dictionary<QuestionEnum, string>() {
            { QuestionEnum.None, "Listen to the Recording" },
            { QuestionEnum.SentenceRepetition, "Sentence Repetition" },
            { QuestionEnum.IntegratedSpeaking, "Integrated Speaking" },
            { QuestionEnum.InteractiveReading, "Interactive Reading" },
            { QuestionEnum.BasicQuestions, "Question" },
            { QuestionEnum.Instructions, "Instructions" }
        };

        public string AnswerOptions { get; set; } = string.Empty;
        public string BasicAnswers1 { get; set; } = string.Empty;
        public string BasicAnswers2 { get; set; } = string.Empty;
        public string BasicAnswers3 { get; set; } = string.Empty;
        public string BasicQuestion1 { get; set; } = string.Empty;
        public string BasicQuestion2 { get; set; } = string.Empty;
        public string BasicQuestion3 { get; set; } = string.Empty;

        [NotMapped]
        public int CurrentQuestionNumber { get; set; }

        public int DurationAnswerInSeconds { get; set; } = 10;
        public int DurationRecordingInSeconds { get; set; } = 60;
        public Guid Guid { get; set; } = Guid.NewGuid();

        public bool HasAudio => Recording != null && Recording.Length > 0;
        public bool HasInteractiveReadingImage => InteractiveReadingImage != null && InteractiveReadingImage.Length > 0;
        public bool HasIntroductionImage => IntroductionImage != null && IntroductionImage.Length > 0;
        public bool HasQuestionImage => QuestionImage != null && QuestionImage.Length > 0;
        public bool HasRecordingImage => RecordingImage != null && RecordingImage.Length > 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string InteractiveReadingAnswer { get; set; } = string.Empty;

        public byte[] InteractiveReadingImage { get; set; } = Array.Empty<byte>();

        public string InteractiveReadingOptions { get; set; } = string.Empty;
        public string InteractiveReadingOptionsAnswerKey { get; set; } = string.Empty;

        public string InteractiveReadingOptionsDropDown { get; set; } = string.Empty;
        public byte[] IntroductionImage { get; set; } = Array.Empty<byte>();

        public string IntroductionText { get; set; } = string.Empty;

        [NotMapped]
        public bool IsIntroduction => QuestionType == QuestionEnum.Instructions || (QuestionType == QuestionEnum.SentenceRepetition && DurationRecordingInSeconds == 0);

        public int OrderBy { get; set; }
        public byte[] QuestionImage { get; set; } = Array.Empty<byte>();
        public virtual QuestionRubric? QuestionRubric { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionTitle => Titles[QuestionType];
        public QuestionEnum QuestionType { get; set; }
        public byte[] Recording { get; set; } = Array.Empty<byte>();
        public byte[] RecordingImage { get; set; } = Array.Empty<byte>();
        public string RecordingText { get; set; } = string.Empty;
        public string Route => (QuestionType == QuestionEnum.Instructions || (DurationAnswerInSeconds == 0 && DurationRecordingInSeconds == 0)) ? "MarkComplete" : DurationAnswerInSeconds == 0 ? "Recording" : "Answer";
        public Test? Test { get; set; }
        public int TestId { get; set; }
        public string Title { get; set; } = string.Empty;

        [NotMapped]
        public int TotalQuestions { get; set; }
    }
}