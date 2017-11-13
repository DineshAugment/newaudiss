using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EndToEnd.Util
{
    public enum EnumMail
    {
        Success = 1,
        Fail = 0
    }

    public static class Common
    {
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static EnumMail SendMail(string strToAddress, string strBody)
        {
            try
            {
                var fromAddress = new MailAddress(ConstHelpers.Mail_FromAddress);
                var toAddress = new MailAddress(strToAddress);
                string strFromPassword = ConstHelpers.Mail_FromAddressPassword;
                string strSubject = ConstHelpers.Mail_Subject;
                string strHTMLBody = strBody;

                var smtp = new SmtpClient()
                {
                    Host = ConstHelpers.Mail_Host,
                    Port = Convert.ToInt32(ConstHelpers.Mail_Port),
                    EnableSsl =Convert.ToBoolean(ConstHelpers.Mail_EnableSSL),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(fromAddress.Address, strFromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = strSubject,
                    Body = strHTMLBody
                })
                {
                    smtp.Send(message);
                }
                return EnumMail.Success;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}