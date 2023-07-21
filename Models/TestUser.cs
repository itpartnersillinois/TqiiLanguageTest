using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class TestUser {
        public int CurrentQuestionId { get; set; }
        public int CurrentQuestionOrder { get; set; }
        public DateTime? DateTimeEnd { get; set; }

        public DateTime? DateTimeStart { get; set; }

        public string Email { get; set; } = string.Empty;

        public Guid Guid { get; set; } = Guid.NewGuid();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderBy { get; set; }

        public string? ReviewerNotes { get; set; }
        public int Score { get; set; }
        public virtual Test? Test { get; set; }
        public int TestId { get; set; }
        public int TotalQuestions { get; set; }
    }
}