using DaoModels.DAO.Models;
using System;

namespace WebDispacher.Service.EmailSmtp
{
    public class PaternSourse
    {
        public string GetPaternDataAccountDriver(string email, string password)
        {
            string patern =  "<h2>Information for entering the application account</h2>" +
                "<br/>" +
                $"<p>Email: {email}</p>" +
                $"<p>Password: {password}</p>" +
                "<p>Link app: https://apps.apple.com/us/app/truckonnow/id1476387303?l=ru&ls=1</p>";
            return patern;
        }
        public string GetPaternDataAccountUser(string email, string password)
        {
            string patern = "<h2>Information for entering the admin panel</h2>" +
                "<br/>" +
                $"<p>Email: {email}</p>" +
                $"<p>Password: {password}</p>" +
                "<p>Link app: http://truckonnow.com/";
            return patern;
        }

        internal string GetPaternNoRestoreDataAccountDriver()
        {
            string patern = "<h2>An attempt was made to change the password, but the attempt was rejected</h2>";
            return patern;
        }

        internal string GetPaternNoRestoreDataAccountUser()
        {
            string patern = "<h2>An attempt was made to change the password, but the attempt was rejected</h2>";
            return patern;
        }

        public string GetPaternRecoveryPassword(string link)
        {
            string patern = $"<p>Your password reset link - {link}</p>";
            return patern;
        }
    }
}