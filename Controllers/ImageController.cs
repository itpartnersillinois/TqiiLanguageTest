using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("image")]
    public class ImageController : Controller {
        private readonly LanguageDbContext _context;

        public ImageController(LanguageDbContext context) {
            _context = context;
        }

        [HttpGet("answer/{id}")]
        public void Answer(string id) {
            var storage = _context?.Questions?.SingleOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.Recording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "image";
            var stream = new MemoryStream(storage.RecordingImage);
            stream.CopyToAsync(Response.Body);
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet("interactivereading/{id}")]
        public void InteractiveReading(string id) {
            var storage = _context?.Questions?.SingleOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.InteractiveReadingImage == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "image";
            var stream = new MemoryStream(storage.InteractiveReadingImage);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("introduction/{id}")]
        public void Introduction(string id) {
            var storage = _context?.Questions?.SingleOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.IntroductionImage == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "image";
            var stream = new MemoryStream(storage.IntroductionImage);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("question/{id}")]
        public void Question(string id) {
            var storage = _context?.Questions?.SingleOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.QuestionImage == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "image";
            var stream = new MemoryStream(storage.QuestionImage);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("recording/{id}")]
        public void Recording(string id) {
            var storage = _context?.Questions?.SingleOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.Recording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "image";
            var stream = new MemoryStream(storage.RecordingImage);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("test")]
        public string Test() {
            return "return value";
        }
    }
}