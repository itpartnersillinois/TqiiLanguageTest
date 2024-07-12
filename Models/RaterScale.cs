using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class RaterScale {
        public string Description { get; set; } = "";
        public string Descriptors { get; set; } = "";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsLocked { get; set; } = false;
        public int Order { get; set; }
        public int? QuestionInformationId { get; set; }
        public string RaterScaleName { get; set; } = "";
        public string Title { get; set; } = "";
        public int Value { get; set; }
        public double Weight { get; set; }
    }
}