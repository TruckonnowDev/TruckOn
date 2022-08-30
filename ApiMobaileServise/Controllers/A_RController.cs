using ApiMobaileServise.Models;
using ApiMobaileServise.Servise;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace ApiMobaileServise.Controllers
{
    [Route("Mobile")]
    public class A_RController : Controller
    {
        ManagerMobileApi managerMobileApi = new ManagerMobileApi();

        [HttpPost]
        [Route("Avtorization")]
        public string Avtorization(string email, string password)
        {
            string token = null;
            string respons = null;
            if ((email != "" || email != null) && (password != "" || password != null))
            {
                token = managerMobileApi.Avtorization(email, password);
                if (token != null && token != "")
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", token));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Password or mail was not correct", ""));
                }
            }
            else
            {
                if (email != "" || email != null)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Email field is empty", ""));
                }
                else if (password != "" || password != null)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Password field is empty", ""));
                }
            }
            return respons;
        }

        [HttpPost]
        [Route("Avtorization/Clear")]
        public string Clear(string token)
        {
            string respons = null;
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    managerMobileApi.ClearToken(token);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", ""));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
                }
            }
            catch (Exception)
            {
                respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Technical work on the service", null));
            }
            return respons;
        }

        [HttpPost]
        [Route("RequestPassword")]
        public string RequestPassword(string email, string fullName)
        {
            string respons = null;
            try
            {
                bool isFullNamePassword = managerMobileApi.CheckFullNameAndPasswrod(email, fullName);
                if (isFullNamePassword)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", ""));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "All fields must be filled ", ""));
                }
            }
            catch (Exception e)
            {
                respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Technical work on the service", null));
            }
            return respons;
        }
    }
}