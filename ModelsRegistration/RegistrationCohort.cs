using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TqiiLanguageTest.ModelsRegistration {

    public class RegistrationCohort {
        public string DateString => $"{StartDate:d}-{EndDate:d}";
        public string Description { get; set; } = string.Empty;

        public DateTime EndDate { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string NumberSlots => NumberStudentsEnrolled >= NumberStudents ? "This session is full -- we will contact you if seats become available" :
            (NumberStudentsEnrolled + 5 >= NumberStudents ? "This session is filling up." : "Seats are available for this session");

        public int NumberStudents { get; set; }

        [NotMapped]
        public int NumberStudentsApplied { get; set; }

        [NotMapped]
        public int NumberStudentsEnrolled { get; set; }

        public virtual ICollection<RegistrationTest>? RegistrationTests { get; set; }
        public DateTime StartDate { get; set; }
        public string TestName { get; set; } = string.Empty;
    }
}