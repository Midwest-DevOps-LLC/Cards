using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Controllers
{
    public class UserController : Controller
    {
        [HttpPost]
        public IActionResult UserInfo(Models.UserModel model)
        {
            DataEntities.ValidationEvent validationModel = new DataEntities.ValidationEvent("Couldn't update", "Couldn't update account");

            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetInt32("UserID") == model.UserID)
                {
                    if (model.ChatName.Length > 30)
                    {
                        model.ChatName = model.ChatName.Substring(0, 30);
                    }

                    if (validationModel.validationStatus == DataEntities.ValidationStatus.Success)
                    {
                        using (var userBLL = new BusinessLogicLayer.Users(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationModel))
                        {
                            var userFromID = userBLL.GetUserByUserID(model.UserID.Value, null);

                            userFromID.ChatName = model.ChatName;
                            userFromID.ModifiedBy = model.UserID.Value;
                            userFromID.ModifiedDate = DateTime.UtcNow;

                            var userID = userBLL.SaveUser(userFromID);

                            if (userID != null)
                            {
                                validationModel.ValidationAlertMessage = "You have successfully updated your account";
                                validationModel.ValidationModalMessage = "Success!";
                            }
                            else
                            {
                                validationModel.ValidationAlertMessage = "Can't update user in database";
                                validationModel.validationStatus = DataEntities.ValidationStatus.Error;
                            }
                        }
                    }
                }
            }

            return Json(validationModel);
        }

        [HttpGet]
        public IActionResult Index()
        {
            DataEntities.ValidationEvent validationModel = new DataEntities.ValidationEvent("Couldn't update", "Couldn't update account");

            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                using (var userBll = new BusinessLogicLayer.Users(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationModel))
                {
                    var user = userBll.GetUserByUserID(HttpContext.Session.GetInt32("UserID").Value, true);

                    if (user != null && user != new DataEntities.User())
                    {
                        return View(new Models.UserModel(user));
                    }
                }

                return NotFound();
            }

            return Unauthorized();
        }
    }
}
