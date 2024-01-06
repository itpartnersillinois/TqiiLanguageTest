using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class CreateRaterTestModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public CreateRaterTestModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        public IList<Tuple<string, string, string>> AssignedRaters { get; set; } = default!;
        public string DateEnded { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int Id { get; set; } = default!;

        public string IdString { get; set; } = default!;
        public IList<RaterName> Raters { get; set; } = default!;
        public string TestName { get; set; } = default!;
        public string UserId { get; set; } = default!;

        public async Task OnGetAsync() {
            var Id = int.Parse(Request.Query["id"]);
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.RaterNames != null) {
                var testinformation = _context.TestUsers.Include(t => t.Test).Select(tu => new { tu.Id, tu.UserIdentification, tu.Email, tu.DateTimeEnd, tu.Test.Title }).First(tu => tu.Id == Id);
                Email = testinformation.Email ?? "";
                TestName = testinformation.Title ?? "";
                DateEnded = testinformation.DateTimeEnd.ToString() ?? "";
                UserId = testinformation.UserIdentification ?? "";
                IdString = Id.ToString();

                var assignedRaterInformation = await _context.RaterTests.Include(rt => rt.Rater).Where(rt => rt.TestUserId == Id).Select(rt => new { rt.Rater.Email, rt.IsExtraScorer, rt.FinalScore }).ToListAsync();

                AssignedRaters = assignedRaterInformation.Select(rt => new Tuple<string, string, string>(rt.Email, rt.IsExtraScorer ? " (Second Pass)" : "", rt.FinalScore == 0 ? "Not Scored" : "Final Score: " + rt.FinalScore)).OrderBy(s => s.Item1).ToList();
                Raters = await _context.RaterNames.ToListAsync();
                Raters = Raters.Where(r => !AssignedRaters.Select(ar => ar.Item1).Contains(r.Email)).ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            var testUserId = int.Parse(Request.Form["id"].ToString());
            var raters = Request.Form.ContainsKey("raters") ? Request.Form["raters"].ToString() : "";
            var ratersSecond = Request.Form.ContainsKey("raters-second") ? Request.Form["raters-second"].ToString() : "";
            int totals = 0;
            if (!string.IsNullOrWhiteSpace(raters)) {
                foreach (var rater in raters.Split(',').Select(r => int.Parse(r.Trim()))) {
                    _context.RaterTests.Add(new RaterTest { DateAssigned = DateTime.Now, IsExtraScorer = false, RaterNameId = rater, TestUserId = testUserId });
                    var rater1 = _context.RaterNames.First(rn => rn.Id == rater);
                    rater1.NumberOfTests++;
                    totals++;
                }
            }
            if (!string.IsNullOrWhiteSpace(ratersSecond)) {
                foreach (var rater in ratersSecond.Split(',').Select(r => int.Parse(r.Trim()))) {
                    _context.RaterTests.Add(new RaterTest { DateAssigned = DateTime.Now, IsExtraScorer = true, RaterNameId = rater, TestUserId = testUserId });
                    var rater2 = _context.RaterNames.First(rn => rn.Id == rater);
                    rater2.NumberOfTests++;
                    totals++;
                }
            }
            var testUser = _context.TestUsers.First(tu => tu.Id == testUserId);
            testUser.NumberReviewers = testUser.NumberReviewers + totals;
            _context.SaveChanges();
            return RedirectToPage("CreateRaterTest", new { id = testUserId });
        }
    }
}