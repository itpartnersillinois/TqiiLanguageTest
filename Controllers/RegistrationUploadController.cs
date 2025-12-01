using Microsoft.AspNetCore.Mvc;

namespace TqiiLanguageTest.Controllers {

    [Route("registration")]
    public class RegistrationUploadController : Controller {

        [HttpGet("assigncourse")]
        public IActionResult Index() {
            return View();
        }
    }
}