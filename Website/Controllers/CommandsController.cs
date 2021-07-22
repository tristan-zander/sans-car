using Microsoft.AspNetCore.Mvc;

namespace SansCar.Controllers
{
    public class CommandsController : Controller
    {
    
        // TODO: Use attributes and stuff to get this information at compile time (source generators?)
        [HttpGet("/api/v1/commands/query-all")]
        public IActionResult QueryAllCommands()
        {
            return BadRequest("Unimplemented");
        }
    }
}