using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class TestUser {
        public int CurrentQuestionId { get; set; }
        public int CurrentQuestionOrder { get; set; }
        public DateTime? DateTimeEnd { get; set; }
        public DateTime? DateTimeExpired { get; set; }

        public DateTime? DateTimeScheduled { get; set; }
        public DateTime? DateTimeStart { get; set; }

        public string Email { get; set; } = string.Empty;
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsPassed { get; set; }
        public string Language { get; set; } = string.Empty;
        public int NumberReviewers { get; set; }
        public int NumberReviewerScores { get; set; }
        public int NumberTimesRefreshed { get; set; } = 0;
        public int OrderBy { get; set; }
        public string? ReviewerNotes { get; set; }
        public float Score { get; set; }
        public virtual Test? Test { get; set; }
        public int TestId { get; set; }
        public int TotalQuestions { get; set; }
        public string? UserIdentification { get; set; }
    }
}