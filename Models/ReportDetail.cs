using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class ReportDetail {
        public string Answer { get; set; } = "";
        public int AnswerId { get; set; }
        public string AnswerKey { get; set; } = "";
        public string AutogradedScore { get; set; } = "";
        public string Email { get; set; } = "";

        public string FinalIndividualNotes { get; set; } = "";
        public int FinalIndividualScore { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsDisqualified { get; set; }

        public bool IsSecondaryRater { get; set; }
        public int NumberOfTimesRefreshed { get; set; }
        public DateTime QuestionAnswered { get; set; }
        public int QuestionId { get; set; }
        public string QuestionName { get; set; } = "";
        public string QuestionType { get; set; } = "";
        public string RaterName { get; set; } = "";
        public string RaterNotes { get; set; } = "";
        public int RaterScore { get; set; }
        public DateTime TestDate { get; set; }
        public string TestName { get; set; } = "";
        public int TestUserId { get; set; }
        public float TotalScore { get; set; }
        public string UserIdentification { get; set; } = "";
    }
}