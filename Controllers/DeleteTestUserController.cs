using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Controllers {

    [Route("deletetestuser")]
    public class DeleteTestUserController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public DeleteTestUserController(PermissionsHandler permissions, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult Index(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var testUser = _context?.TestUsers?.SingleOrDefault(tu => tu.Id == id);
            if (testUser == null) {
                return NotFound();
            }
            var answers = _context?.Answers?.Where(a => a.TestUserId == id).ToList() ?? new List<Answer>();
            _context?.RemoveRange(answers);
            _context?.Remove(testUser);
            _context?.SaveChanges();
            return Ok();
        }

        [HttpGet("rewind/{id}")]
        public IActionResult Rewind(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                return Unauthorized();
            }
            var testUser = _context?.TestUsers?.SingleOrDefault(tu => tu.Id == id);
            if (testUser == null) {
                return NotFound();
            }
            testUser.CurrentQuestionOrder = testUser.CurrentQuestionOrder == 0 ? 0 : testUser.CurrentQuestionOrder - 1;
            _context?.SaveChanges();
            return Ok();
        }
    }
}