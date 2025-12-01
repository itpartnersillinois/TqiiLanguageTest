using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Data {

    public class RegistrationDbContext : DbContext {
        private readonly Guid _id;

        public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options) : base(options) {
            _id = Guid.NewGuid();
            Debug.WriteLine($"{_id} context created.");
        }

        public DbSet<RegistrationCohortPerson>? CohortPeople { get; set; }
        public DbSet<RegistrationCohort>? Cohorts { get; set; }
        public DbSet<RegistrationDocument>? Documents { get; set; }
        public DbSet<RegistrationInstruction>? Instructions { get; set; }
        public DbSet<RegistrationPerson>? People { get; set; }
        public DbSet<RegistrationTestPerson>? RegistrationTestPeople { get; set; }
        public DbSet<RegistrationTest>? RegistrationTests { get; set; }

        public override void Dispose() {
            Debug.WriteLine($"{_id} context disposed.");
            base.Dispose();
        }

        public override ValueTask DisposeAsync() {
            Debug.WriteLine($"{_id} context disposed async.");
            return base.DisposeAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            Debug.WriteLine($"{_id} context starting initial setup.");
            modelBuilder.Entity<RegistrationTestPerson>()
                .HasOne(e => e.RegistrationTest)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RegistrationTestPerson>()
                .HasOne(e => e.RegistrationCohortPerson)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            Debug.WriteLine($"{_id} context finishing initial setup.");
        }
    }
}