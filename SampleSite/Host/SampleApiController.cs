using Microsoft.AspNetCore.Mvc;

namespace SampleSite.Host
{
    public class SampleApiController : Controller
    {
        [HttpGet("/api/notfound")]
        public IActionResult HttpNotFound()
        {
            return NotFound("This is message.");
        }
    }
}
