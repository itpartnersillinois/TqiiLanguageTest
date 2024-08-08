using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class RaterAnswer {
        public virtual Answer? Answer { get; set; }
        public int AnswerId { get; set; }
        public string AutogradedNotes { get; set; } = "";
        public DateTime? DateFinished { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsAnswered { get; set; }
        public bool IsDisqualified { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsSuspicious { get; set; }

        public string Notes { get; set; } = "";
        public virtual RaterTest? RaterTest { get; set; }
        public int RaterTestId { get; set; }
        public float Score { get; set; }
        public string ScoreText { get; set; } = "";
    }
}