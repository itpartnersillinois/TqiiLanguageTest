using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.Email {

    public class RegistrationEmail {
        private readonly RegistrationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly InstructionHelper _instructionHelper;

        public RegistrationEmail(RegistrationDbContext context, IEmailSender emailSender, InstructionHelper instructionHelper) {
            _context = context;
            _emailSender = emailSender;
            _instructionHelper = instructionHelper;
        }

        public async Task<string> SendEmails(int cohortId, bool sendAll) {
            var cohort = _context.Cohorts?.SingleOrDefault(c => c.Id == cohortId);
            if (cohort == null) {
                return "Cohort not found.";
            }
            var approved = _instructionHelper.GetInstructionString(InstructionType.EmailApproved);
            var denied = _instructionHelper.GetInstructionString(InstructionType.EmailDenied);
            var waitlisted = _instructionHelper.GetInstructionString(InstructionType.Waitlisted);
            var cohortPeople = _context.CohortPeople?.Include(cp => cp.RegistrationCohort).Include(cp => cp.RegistrationPerson).Where(cp => cp.RegistrationCohortId == cohortId && cp.DateRegistrationSent == null).ToList() ?? new List<RegistrationCohortPerson>();
            var count = 0;
            var countSkipped = 0;
            foreach (var cohortPerson in cohortPeople) {
                var body = $"<p>Dear {cohortPerson.RegistrationPerson?.FirstName},</p>";
                var sendEmail = true;
                if (cohortPerson.IsApproved) {
                    body += $"<p>Congratulations! You have been approved to participate in the {cohort?.TestName} starting on {cohort?.StartDate.ToString("MMMM dd, yyyy")}.</p>";
                    body += "<p>Please follow the instructions below to complete your registration:</p>";
                    body += "<ul>";
                    var tests = _context.RegistrationTestPeople?.Include(tp => tp.RegistrationTest).Where(tp => tp.RegistrationCohortPersonId == cohortPerson.Id).ToList() ?? new List<RegistrationTestPerson>();
                    foreach (var test in tests) {
                        if (test.IsProficiencyExemptionApproved) {
                            body += $"<li>You have been granted an exemption for the {test.RegistrationTest?.TestName}.</li>";
                        } else if (!string.IsNullOrWhiteSpace(test.RegistrationTest?.RegistrationLink)) {
                            body += $"<li>Register for the {test.RegistrationTest?.TestName} by visiting the following link: <a href='{test.RegistrationTest?.RegistrationLink}'>{test.RegistrationTest?.RegistrationLink}</a></li>";
                        } else {
                            body += $"<li>Register for the {test.RegistrationTest?.TestName} will be managed by the test administrator.</li>";
                        }
                    }
                    body += "</ul>";
                    body += approved;
                    cohortPerson.DateRegistered = DateTime.UtcNow;
                    cohortPerson.DateRegistrationSent = DateTime.UtcNow;
                    _context.Update(cohortPerson);
                } else if (cohortPerson.IsDenied) {
                    body += $"<p>We regret to inform you that your application for the {cohort?.TestName} starting on {cohort?.StartDate.ToString("MMMM dd, yyyy")} has been denied.</p>";
                    body += denied;
                    cohortPerson.DateRegistrationSent = DateTime.UtcNow;
                    _context.Update(cohortPerson);
                } else if (cohortPerson.IsWaitlisted) {
                    body += $"<p>You have been placed on the waitlist for the {cohort?.TestName} starting on {cohort?.StartDate.ToString("MMMM dd, yyyy")}.</p>";
                    body += "<p>We will notify you if a spot becomes available.</p>";
                    body += waitlisted;
                    cohortPerson.DateRegistrationSent = DateTime.UtcNow;
                    _context.Update(cohortPerson);
                } else if (sendAll) {
                    body += "<p>Your application is still under review. We will notify you once a decision has been made.</p>";
                } else {
                    sendEmail = false;
                }
                _ = await _context.SaveChangesAsync();
                body += $"<p>{cohortPerson.ExternalComment}</p>";
                if (sendEmail) {
                    count++;
                    await _emailSender.SendEmailAsync(cohortPerson.RegistrationPerson?.Email ?? "", "TQII Registration", body);
                } else {
                    countSkipped++;
                }
            }
            return $"{count} emails sent. {countSkipped} emails skipped.";
        }
    }
}