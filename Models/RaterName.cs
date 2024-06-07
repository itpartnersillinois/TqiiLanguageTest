using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class RaterName {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsActive { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int NumberOfTests { get; set; }
    }
}