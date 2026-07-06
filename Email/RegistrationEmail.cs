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

        public async Task<string> SendConfirmationEmail(int cohortPersonId) {
            var cohortPerson = _context.CohortPeople?.Include(cp => cp.RegistrationCohort).Include(cp => cp.RegistrationPerson).SingleOrDefault(cp => cp.Id == cohortPersonId);
            if (cohortPerson == null) {
                return "Cohort person not found.";
            }
            var completion = _instructionHelper.GetInstructionString(InstructionType.EmailOnCompletion);
            var tests = _context.RegistrationTestPeople?.Include(tp => tp.RegistrationTest).Where(tp => tp.RegistrationCohortPersonId == cohortPerson.Id).ToList() ?? new List<RegistrationTestPerson>();

            var body = $"<p>Dear {cohortPerson.RegistrationPerson?.FirstName},</p>";
            body += $"<p>Thank you for registering for the {cohortPerson.RegistrationCohort?.TestName} starting on {cohortPerson.RegistrationCohort?.StartDate.ToString("MMMM dd, yyyy")}.</p>";
            body += completion;
            body += "<p>Test details</p><ul>";
            foreach (var test in tests) {
                if (test.IsProficiencyExemption) {
                    body += $"<li>You have requested an exemption for the {test.RegistrationTest?.TestName}{(string.IsNullOrWhiteSpace(test.Language) ? "" : $" ({test.Language})")}.</li>";
                } else
                    body += $"<li>You have enrolled for the {test.RegistrationTest?.TestName}{(string.IsNullOrWhiteSpace(test.Language) ? "" : $" ({test.Language})")}.</li>";
            }
            body += "</ul>";

            await _emailSender.SendEmailAsync(cohortPerson.RegistrationPerson?.Email ?? "", "TQII Registration Confirmation", body);
            return "";
        }

        public async Task<string> SendEmails(int cohortId, bool sendAll) {
            var cohort = _context.Cohorts?.SingleOrDefault(c => c.Id == cohortId);
            if (cohort == null) {
                return "Cohort not found.";
            }
            var approved = _instructionHelper.GetInstructionString(InstructionType.EmailApproved);
            var denied = _instructionHelper.GetInstructionString(InstructionType.EmailDenied);
            var waitlisted = _instructionHelper.GetInstructionString(InstructionType.Waitlisted);
            var cohortPeople = _context.CohortPeople?.Include(cp => cp.RegistrationCohort).Include(cp => cp.RegistrationPerson).Where(cp => cp.RegistrationCohortId == cohortId && cp.IsRegistrationCompleted && cp.DateRegistrationSent == null).ToList() ?? new List<RegistrationCohortPerson>();
            var count = 0;
            var countSkipped = 0;
            foreach (var cohortPerson in cohortPeople) {
                var body = $"<p>Dear {cohortPerson.RegistrationPerson?.FirstName},</p>";
                var sendEmail = true;
                if (cohortPerson.IsApproved) {
                    body += $"<p>Congratulations! You have been approved to participate in {cohort?.TestName} starting on {cohort?.StartDate.ToString("MMMM dd, yyyy")}. Please see the details below regarding your module approval status and the remaining training and testing requirements:</p>";
                    var instructions = "";
                    var tests = _context.RegistrationTestPeople?.Include(tp => tp.RegistrationTest).Where(tp => tp.RegistrationCohortPersonId == cohortPerson.Id).ToList() ?? new List<RegistrationTestPerson>();
                    foreach (var test in tests) {
                        if (test.IsProficiencyExemptionApproved) {
                            instructions += $"<li>{test.RegistrationTest?.TestName}: exempted.</li>";
                        } else if (test.RegistrationTest?.RegistrationLink == "admin" || string.IsNullOrWhiteSpace(test.RegistrationTest?.RegistrationLink)) {
                            instructions += $"<li>{test.RegistrationTest?.TestName}: required.</li>";
                        } else {
                            instructions += $"<li><a href='{test.RegistrationTest?.RegistrationLink}'>{test.RegistrationTest?.TestName}</a>: required</li>";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(instructions)) {
                        body += "<ul>";
                        body += instructions;
                        body += "</ul>";
                    }
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