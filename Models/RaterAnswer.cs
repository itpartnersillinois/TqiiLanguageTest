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

        public string Notes { get; set; } = "";
        public virtual RaterTest? RaterTest { get; set; }
        public int RaterTestId { get; set; }
        public int Score { get; set; }
    }
}