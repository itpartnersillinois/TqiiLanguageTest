using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.Models {

    public class Permission {
        public string Email { get; set; } = "";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsItemWriter { get; set; }

        public bool IsReviewer { get; set; }
    }
}