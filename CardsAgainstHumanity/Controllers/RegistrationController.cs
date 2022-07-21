using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Controllers
{
    public class RegistrationController : Controller
    {
        [HttpGet]
        public IActionResult SendNewEmailValidation(string userName)
        {
            var f = PostRequest(userName);

            TempData["VerifiedEmail"] = "Verification sent to your email";
            return RedirectToAction("Index", "Login");
        }

        internal static string PostRequest(string userName)
        {
            string html = string.Empty;
            string url = "https://www.midwestdevops.com/Registration/SendNewEmailValidation?userName=" + userName;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }

        internal static string PostRequestEmail(string UUID)
        {
            string html = string.Empty;
            string url = "https://www.midwestdevops.com/Registration/Email?UUID=" + UUID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }

        [HttpGet]
        public IActionResult Email(Models.EmailRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.UUID))
                {
                    var s = PostRequestEmail(model.UUID);

                    TempData["VerifiedEmail"] = "You have successfully verified your email";
                    return RedirectToAction("Index", "Login");
                }
            }

            return NotFound();
        }
    }
}
