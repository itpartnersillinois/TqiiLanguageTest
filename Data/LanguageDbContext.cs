using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Data {

    public class LanguageDbContext : DbContext {
        private readonly Guid _id;

        public LanguageDbContext(DbContextOptions<LanguageDbContext> options) : base(options) {
            _id = Guid.NewGuid();
            Debug.WriteLine($"{_id} context created.");
        }

        public DbSet<Answer>? Answers { get; set; }
        public DbSet<LanguageOptions>? LanguageOptions { get; set; }
        public DbSet<Permission>? Permissions { get; set; }
        public DbSet<Question>? Questions { get; set; }
        public DbSet<RaterAnswer>? RaterAnswers { get; set; }
        public DbSet<RaterName>? RaterNames { get; set; }
        public DbSet<RaterScale>? RaterScales { get; set; }
        public DbSet<RaterTest>? RaterTests { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }
        public DbSet<ReportIsbe> ReportIsbes { get; set; }
        public DbSet<Test>? Tests { get; set; }
        public DbSet<TestUser>? TestUsers { get; set; }
        public DbSet<User>? Users { get; set; }

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
            Debug.WriteLine($"{_id} context finishing initial setup.");
        }
    }
}