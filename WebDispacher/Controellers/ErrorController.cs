using Microsoft.AspNetCore.Mvc;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("error")]
        public IActionResult ExudeError(int code)
        {
            ViewData["TypeNavBar"] = "BaseAllUsers";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            IActionResult actionResult = null;
            if(code >= 400 && code < 500)
            {
                actionResult = View("Error404");
            }
            else
            {
                actionResult = View("Error500");
            }
            return actionResult;
        }

        public IActionResult Error()
        {
            ViewData["TypeNavBar"] = "BaseAllUsers";
            return View("Error500");
        }
    }
}
