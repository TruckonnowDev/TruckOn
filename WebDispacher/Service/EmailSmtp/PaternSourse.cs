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
            string pattern = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n  <head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n</head>\r\n\r\n<body bgcolor=\"#e1e5e8\" style=\"margin-top:0 ;margin-bottom:0 ;margin-right:0 ;margin-left:0 ;padding-top:0px;padding-bottom:0px;padding-right:0px;padding-left:0px;background-color:#e1e5e8;\">\r\n  <center style=\"width:100%;table-layout:fixed;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;background-color:#e1e5e8;\">\r\n    <div style=\"max-width:600px;margin-top:0;margin-bottom:0;margin-right:auto;margin-left:auto;\">\r\n      <table align=\"center\" cellpadding=\"0\" style=\"border-spacing:0;color:#333333;font-family:'Muli',Arial,sans-serif;Margin:0 auto;width:100%;max-width:600px;\">\r\n        <tbody>\r\n          <tr>\r\n            <td align=\"center\" height=\"143\" style=\"padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;height:143px;vertical-align:middle;\" valign=\"middle\">\r\n              <span class=\"sg-image\">\r\n              <a href=\"https://truckonnow.com/\">\r\n                <img src=\"https://drive.google.com/uc?export=view&id=1Qy_FQukSNIiPy3_4nPKp5FMbhbh88SaO\" alt=\"Logo\" title=\"Logo\" style=\"display:block\" width=\"200\"/>\r\n              </a>\r\n            </span>\r\n          </td>\r\n          </tr>\r\n          <tr>\r\n            <td class=\"one-column\" style=\"padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;background-color:#ffffff;\">\r\n              <table style=\"border-spacing:0;\" width=\"100%\">\r\n                <tbody>\r\n                  <tr>\r\n                    <td class=\"inner contents center\" style=\"padding-top:15px;padding-bottom:15px;padding-right:30px;padding-left:30px;text-align:left;\">\r\n                      <center>\r\n                        <p class=\"h1 center\" style=\"text-align:center;font-weight:100;font-size:26px;Margin-bottom:26px;margin-top:20px;\">Password Reset</p>\r\n                        <p class=\"description center\" style=\"Margin:0;text-align:center;max-width:380px;color:#a1a8ad;line-height:24px;font-size:14px;Margin-bottom:20px;margin-left: auto; margin-right: auto;\">\r\n                          <span style=\"color: rgb(37, 38, 39); font-size: 15px; text-align: center; background-color: rgb(255, 255, 255);\">It seems like you forgot your password for Tuckonnow.<br/>If this is true, click the link below to reset your password.</span>\r\n                        </p>\r\n\r\n                        <span class=\"sg-image\">\r\n                          <a href=\"{link}\" target=\"_blank\">\r\n                            <button style=\"background:#2c5deb;color:#F9f9f9;font-style:normal;font-weight:600;font-size:14px;border-radius:10px;padding:12px 22px;border:1px solid #2c5deb;cursor:pointer;\">Reset Your Password</button>\r\n                          </a>\r\n                        </span>\r\n                        <p class=\"description center\" style=\"Margin:0;text-align:center;max-width:370px;color:#a1a8ad;line-height:24px;font-size:14px;Margin-top:10px;Margin-bottom:10px;margin-left: auto; margin-right: auto;\">\r\n                          <span style=\"color: rgb(161, 168, 173); font-size: 12px; text-align: center; background-color: rgb(255, 255, 255);\">If you did not request a new password, please disregard this email.\r\n                            <br/>\r\n                          </span>\r\n                        </p>\r\n                        <p class=\"description center\" style=\"Margin:0;text-align:left;max-width:360px;color:#a1a8ad;line-height:24px;font-size:12px;Margin-top:20px;Margin-bottom:10px;margin-left: auto; margin-right: auto;\">\r\n                          <span style=\"text-align: left;\">Sincerely Yours,<br/>The Truckonnow team.</span>\r\n                        </p>\r\n                      </center>\r\n                    </td>\r\n                  </tr>\r\n                </tbody>\r\n              </table>\r\n            </td>\r\n          </tr>\r\n          <tr>\r\n            <td height=\"40\">\r\n              <p style=\"line-height: 40px; padding: 0 0 0 0; margin: 0 0 0 0;\">&nbsp;</p>\r\n              <p>&nbsp;</p>\r\n            </td>\r\n          </tr>\r\n        </tbody>\r\n      </table>\r\n    </div>\r\n  </center>\r\n  <!--[if gte mso 9]>\r\n</td></tr></table>\r\n</center>\r\n<![endif]-->\r\n\r\n\r\n</body>\r\n</html>";
            return pattern;
        }

        public string GetPatternRegistrationMail()
        {
            string pattern = $"<!DOCTYPE html><html lang=\"en\"> <head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"> <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"></head><body bgcolor=\"#e1e5e8\" style=\"margin-top:0 ;margin-bottom:0 ;margin-right:0 ;margin-left:0 ;padding-top:0px;padding-bottom:0px;padding-right:0px;padding-left:0px;background-color:#e1e5e8;\"> <center style=\"width:100%;table-layout:fixed;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;background-color:#e1e5e8;\"> <div style=\"max-width:600px;margin-top:0;margin-bottom:0;margin-right:auto;margin-left:auto;\"> <table align=\"center\" cellpadding=\"0\" style=\"border-spacing:0;color:#333333;font-family:'Muli',Arial,sans-serif;Margin:0 auto;width:100%;max-width:600px;\"> <tbody> <tr> <td align=\"center\" height=\"143\" style=\"padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;height:143px;vertical-align:middle;\" valign=\"middle\"> <span class=\"sg-image\"> <a href=\"https://truckonnow.com/\"> <img src=\"https://drive.google.com/uc?export=view&id=1Qy_FQukSNIiPy3_4nPKp5FMbhbh88SaO\" alt=\"Logo\" title=\"Logo\" style=\"display:block\" width=\"200\"/> </a> </span> </td></tr><tr> <td class=\"one-column\" style=\"padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;background-color:#ffffff;\"> <table style=\"border-spacing:0;\" width=\"100%\"> <tbody> <tr> <td class=\"inner contents center\" style=\"padding-top:15px;padding-bottom:15px;padding-right:30px;padding-left:30px;text-align:left;\"> <center> <p class=\"description center\" style=\"Margin:0;text-align:left;max-width:380px;color:#a1a8ad;line-height:24px;font-size:14px;Margin-bottom:20px;margin-left: auto; margin-right: auto;\"> <span style=\"color: rgb(37, 38, 39); font-size: 15px; background-color: rgb(255, 255, 255);\"> Dear Client, <br/> It is our great pleasure to welcome you to Truckonnow. We look forward to continuing our journey together for many years to come!</span> </p><span class=\"sg-image\"> <a href=\"https://truckonnow.com/carrier-login\" target=\"_blank\"> <button style=\"background:#2c5deb;color:#F9f9f9;font-style:normal;font-weight:600;font-size:14px;border-radius:10px;padding:12px 22px;border:1px solid #2c5deb;cursor:pointer;\">Login To Your Account</button> </a> </span> <p class=\"description center\" style=\"Margin:0;text-align:left;max-width:370px;color:#a1a8ad;line-height:24px;font-size:14px;Margin-top:10px;Margin-bottom:10px;margin-left: auto; margin-right: auto;\"> <span style=\"color: rgb(161, 168, 173); font-size: 12px; background-color: rgb(255, 255, 255);\">We are here to help, please feel free to talk with one of our experts, call +1 (773) 420-4444 or email us at sales@truckonnow.com <br/> </span> </p><p class=\"description center\" style=\"Margin:0;text-align:left;max-width:360px;color:#a1a8ad;line-height:24px;font-size:12px;Margin-top:20px;Margin-bottom:10px;margin-left: auto; margin-right: auto;\"> <span style=\"text-align: left;\">With our warmest regards,<br/>Truckonnow team.</span> </p><span class=\"image-team\"> <img src=\"https://www.accreditedlanguage.com/wp-content/uploads/2015/10/photodune-431974-business-team-m.jpg\" alt=\"team photo\" style=\"max-width: 380px;height: auto;\"> </span> </center> </td></tr></tbody> </table> </td></tr><tr> <td height=\"40\"> <p style=\"line-height: 40px; padding: 0 0 0 0; margin: 0 0 0 0;\">&nbsp;</p><p>&nbsp;</p></td></tr></tbody> </table> </div></center><!--[if gte mso 9]></td></tr></table></center><![endif]--></body></html>";

            return pattern;
        }
    }
}