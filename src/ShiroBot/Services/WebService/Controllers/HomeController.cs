using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ShiroBot.Services.WebService.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        // GET: ~/
        // View() will create a result from www/Views/Home/Index.cshtml
        [Route("")]
        [HttpGet("~/")]
        public ActionResult Index() => View();
    }
}