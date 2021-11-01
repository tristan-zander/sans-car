using System;
using Microsoft.AspNetCore.Mvc;

namespace SansCar.Controllers
{
    public class MiscController : Controller
    {

        // Redirect to Discord's OAuth 2.0 page
        [HttpGet("/add-bot")]
        public IActionResult AddBot()
        {
            return Redirect("https://discord.com/api/oauth2/authorize?client_id=688911764703674431&permissions=36817984&redirect_uri=https%3A%2F%2Fsanscar.net%2Fuserauth%2Fdiscord-cb&response_type=code&scope=identify%20email%20bot");
        }

        [HttpGet("/userauth/discord-cb")]
        public IActionResult DiscordOAuthCallback(string data)
        {
            return Ok("Successfully authorized your account.");
        }
    }
}