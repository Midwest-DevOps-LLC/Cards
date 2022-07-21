using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Registration = TempData["VerifiedEmail"];

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("UserAuthID");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Login(Models.LoginModel model)
        {
            var f = PostRequest("https://www.midwestdevops.com/Login/Login", model);

            var validationModel = Newtonsoft.Json.JsonConvert.DeserializeObject<DataEntities.ValidationEvent>(f);

            if (validationModel.validationStatus == DataEntities.ValidationStatus.Success)
            {
                //check if user id is in database if not create new record

                using (var userBLL = new BusinessLogicLayer.Users(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationModel))
                {
                    var userName = validationModel.validationMessages.Where(x => x.HTMLID == "Username").Select(x => x.Message).FirstOrDefault();
                    var userAuthID = Convert.ToInt32(validationModel.validationMessages.Where(x => x.HTMLID == "UserID").Select(x => x.Message).FirstOrDefault());
                    var userID = 0;

                    var userFromDB = userBLL.GetUserByUserAuthID(userAuthID, null);

                    if (userFromDB == null)
                    {
                        var newUser = new Models.UserModel()
                        {
                            UserAuthID = userAuthID,
                            Username = userName,
                            Active = true,
                            CreatedBy = 0,
                            ModifiedBy = 0,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        };

                        var saveUser = userBLL.SaveUser(newUser.ConvertToEntity());

                        if (saveUser == null)
                        {
                            validationModel.ValidationAlertMessage = "Couldn't copy user";
                            validationModel.Add(new DataEntities.ValidationMessage("username", "", DataEntities.ValidationStatus.Error));
                        }
                        else
                        {
                            userFromDB.Username = userName; //Update username from MDO

                            var updateUser = userBLL.SaveUser(userFromDB);

                            userID = Convert.ToInt32(saveUser.Value);
                        }
                    }
                    else
                    {
                        userID = Convert.ToInt32(userFromDB.UserID.Value);
                    }

                    if (validationModel.validationStatus == DataEntities.ValidationStatus.Success)
                    {
                        HttpContext.Session.SetString("Username", userName);
                        HttpContext.Session.SetInt32("UserID", userID);
                        HttpContext.Session.SetInt32("UserAuthID", userAuthID);
                    }
                }
            }

            return Json(validationModel);
        }

        internal static CookieContainer cookieContainer = new CookieContainer();
        internal static string PostRequest(string url, Models.LoginModel model)
        {
            string result = "";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.CookieContainer = cookieContainer;

            var postData = "Username=" + model.Username;
            postData += "&Password=" + model.Password;
            var data = Encoding.ASCII.GetBytes(postData);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = data.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        [HttpPost]
        public IActionResult Register(Models.RegisterModel model)
        {
            var f = PostRequest("https://www.midwestdevops.com/Login/Register", model);

            var validationModel = Newtonsoft.Json.JsonConvert.DeserializeObject<DataEntities.ValidationEvent>(f);

            return Json(validationModel);
        }

        internal static string PostRequest(string url, Models.RegisterModel model)
        {
            string result = "";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.CookieContainer = cookieContainer;

            var postData = "Username=" + model.Username;
            postData += "&Password=" + model.Password;
            postData += "&Email=" + model.Email;
            postData += "&RetypePassword=" + model.RetypePassword;
            postData += "&Application=" + model.Application;
            var data = Encoding.ASCII.GetBytes(postData);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = data.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
