using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class User {
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public DateTime? DateLastTest { get; set; }
        public string Email { get; set; } = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsPracticeTestEnded { get; set; } = false;
        public bool IsPracticeTestStarted { get; set; } = false;
        public string Language { get; set; } = string.Empty;
    }
}