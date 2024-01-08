using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class RaterTest {
        public DateTime? DateAssigned { get; set; }
        public DateTime? DateFinished { get; set; }
        public float FinalScore { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsExtraScorer { get; set; }
        public bool IsPassed { get; set; }
        public string Notes { get; set; } = "";
        public virtual RaterName? Rater { get; set; }
        public int RaterNameId { get; set; }
        public virtual TestUser? Test { get; set; }
        public int TestUserId { get; set; }
    }
}