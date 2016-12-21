using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ShiroBot.Services.WebService.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        // GET: ~/
        [Route("")]
        [HttpGet("~/")]
        public ActionResult Index() => View();
    }
}