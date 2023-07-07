using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;

namespace WebDispacher.Controellers
{
    public class BaseController : Controller
    {
        internal readonly IUserService userService;

        public BaseController(IUserService userService)
        {
            this.userService = userService;
        }

        public bool CheckPermissionsByCookies(string route, out string key, out string idCompany)
        {
            Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var keyCookie);
            Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompanyCookie);

            key = keyCookie;
            idCompany = idCompanyCookie;

            var result = userService.CheckPermissions(keyCookie, idCompanyCookie, route);

            return result;
        }

        public string GetCookieCompanyName() 
        {
            Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);

            return companyName;
        }
    }
}
