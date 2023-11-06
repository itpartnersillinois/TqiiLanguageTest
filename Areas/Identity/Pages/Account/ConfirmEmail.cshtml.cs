// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TqiiLanguageTest.Areas.Identity.Pages.Account {

    public class ConfirmEmailModel : PageModel {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, IEmailSender emailSender) {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code) {
            if (userId == null || code == null) {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            if (result.Succeeded) {
                await _emailSender.SendEmailAsync(user.Email, "Welcome to the TQII English Proficiency Test", $"<p>Hello,</p><p>Thank you for registering for the Training Qualified Interpreters (TQII) English Proficiency Test. As an integral component of the TQII program, achieving success in this test will affirm the strength of your language ability for becoming a qualified interpreter.</p><p>The TQII Language test has three modules, and all items within each module are timed. The first module is \"Sentence Repetition,\" the second module is \"Integrated Speaking,\" and the third module is \"Interactive Reading.\"</p><p>We understand that some aspects of the TQII English Proficiency Test may be new to you. To ensure you are fully prepared, we offer a practice session to acquaint you with the test format. This session will include a comprehensive overview and guided exploration of the tasks you will encounter. It is an excellent opportunity for you to become comfortable with the test structure, enabling you to approach the actual test with confidence. Following the completion of your practice session, you will be promptly directed to the actual test.</p><p><a href='https://tqii-language.education.illinois.edu/'>Please visit our website</a> to access the practice session and for more details on the testing process. We are here to support you every step of the way, so feel free to reach out to tqii.uiuc@gmail.com if you have any questions or need further assistance.</p><p>Thank you for your dedication to becoming a qualified interpreter through the TQII program. We look forward to seeing your language skills in action.</p><p>Warm regards,</p><p>TQII Language Proficiency Test Team");
            }

            return Page();
        }
    }
}