using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("testmedia")]
    public class TestMediaController : Controller {
        private readonly LanguageDbContext _context;

        public TestMediaController(LanguageDbContext context) {
            _context = context;
        }

        [HttpGet("conclusion/{id}")]
        public void Conclusion(string id) {
            var storage = _context?.Tests?.FirstOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.ConclusionRecording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "audio/ogg; codecs=opus";
            var stream = new MemoryStream(storage.ConclusionRecording);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("introduction/{id}")]
        public void Introduction(string id) {
            var storage = _context?.Tests?.FirstOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.IntroductionRecording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "audio/ogg; codecs=opus";
            var stream = new MemoryStream(storage.IntroductionRecording);
            stream.CopyToAsync(Response.Body);
        }
    }
}