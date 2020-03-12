using Microsoft.AspNetCore.Mvc;

namespace SampleSite.Server
{
    public class SampleApiController : Controller
    {
        [HttpGet("/api/notfound.xml")]
        public IActionResult HttpNotFound()
        {
            return NotFound("This is message.");
        }
    }
}
