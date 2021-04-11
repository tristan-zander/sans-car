using Microsoft.AspNetCore.Mvc;

namespace SansCar.Controllers
{
    public class CommandsController : Controller
    {
        [HttpGet("/api/v1/commands/query-all")]
        public IActionResult QueryAllCommands()
        {
            return BadRequest("Unimplemented");
        }
    }
}