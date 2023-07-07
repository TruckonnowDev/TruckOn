using Microsoft.AspNetCore.Mvc;
using WebDispacher.Constants;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("error")]
        public IActionResult ExudeError(int code)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.Error;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if(code >= 400 && code < 500)
            {
                return View("Error404");
            }

            return  View("Error500");
        }

        [HttpGet]
        [Route("Notready")]
        public IActionResult NotReadyPage()
        {
            return View("NotReady");
        }
    }
}
