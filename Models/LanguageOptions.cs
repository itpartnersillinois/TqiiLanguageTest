using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class LanguageOptions {
        public string Characters { get; set; } = string.Empty;

        public bool EnforceStrictGrading { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Language { get; set; } = string.Empty;
        public bool Popout { get; set; }
    }
}