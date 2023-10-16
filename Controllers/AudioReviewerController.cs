using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("audioreviewer")]
    public class AudioReviewerController : Controller {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public AudioReviewerController(PermissionsHandler permissions, LanguageDbContext context) {
            _permissions = permissions;
            _context = context;
        }

        [HttpGet("answer/{id}")]
        public void Index(int id) {
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            var storage = _context?.Answers?.SingleOrDefault(a => a.Id == id);
            if (storage == null || storage.Recording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "audio/ogg; codecs=opus";
            var stream = new MemoryStream(storage.Recording);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("question/{id}")]
        public void Question(int id) {
            if (!_permissions.IsReviewer(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            var storage = _context?.Questions?.SingleOrDefault(q => q.Id == id);
            if (storage == null || storage.Recording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "audio/ogg; codecs=opus";
            var stream = new MemoryStream(storage.Recording);
            stream.CopyToAsync(Response.Body);
        }
    }
}