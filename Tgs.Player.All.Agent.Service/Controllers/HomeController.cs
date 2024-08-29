using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Tgs.Player.All.Agent.Service.Controllers
{
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [Route("/")]
        [HttpGet]
        [Description("Home")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return Ok("Player All Agent API");
        }
    }
}
