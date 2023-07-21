using Microsoft.AspNetCore.Mvc;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.Controllers {

    [Route("media")]
    public class MediaController : Controller {
        private readonly LanguageDbContext _context;

        public MediaController(LanguageDbContext context) {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet("{id}")]
        public void Index(string id) {
            var storage = _context?.Questions?.SingleOrDefault(q => q.Guid == Guid.Parse(id));
            if (storage == null || storage.Recording == null) {
                Response.StatusCode = 404;
                return;
            }
            Response.StatusCode = 200;
            Response.ContentType = "audio/ogg; codecs=opus";
            var stream = new MemoryStream(storage.Recording);
            stream.CopyToAsync(Response.Body);
        }

        [HttpGet("test")]
        public string Test() {
            return "return value";
        }
    }
}