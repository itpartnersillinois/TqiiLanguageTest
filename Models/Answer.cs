using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class Answer {
        public DateTime? DateTimeEnd { get; set; }

        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeTextAnswered { get; set; } = DateTime.Now;
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderBy { get; set; }
        public virtual Question? Question { get; set; }
        public virtual QuestionRubric? QuestionRubric { get; set; }
        public byte[] Recording { get; set; } = Array.Empty<byte>();
        public string? ReviewerNotes { get; set; }
        public string? RubricInformation { get; set; }
        public virtual TestUser? TestUser { get; set; }
        public string Text { get; set; } = "";
    }
}