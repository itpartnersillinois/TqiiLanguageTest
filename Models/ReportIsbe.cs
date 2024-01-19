using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class ReportIsbe {
        public string Email { get; set; } = "";

        public DateTime? FinalizedDate { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public float IntegratedSpeakingScore { get; set; }
        public float InteractiveReadingScore { get; set; }
        public bool IsPassed { get; set; } = false;
        public float SentenceRepetitionScore { get; set; }
        public DateTime TestDate { get; set; }
        public string TestName { get; set; } = "";
        public float TotalScore { get; set; }
        public string UserIdentification { get; set; } = "";
    }
}