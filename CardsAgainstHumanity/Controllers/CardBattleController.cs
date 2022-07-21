using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstHumanity.Controllers
{
    public class CardBattleController : Controller
    {
        //todo deck viewer
        //deck creator with card creator inside

        public IActionResult Index()
        {
            var index = new DataEntities.CardBattle.CardBattleIndex();

            using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var rankings = cardBattleBLL.GetAllCardBattleRanking();

                index.WhiteCards = rankings;
            }

            return View(index);
        }

        public IActionResult CardBattle()
        {
            var cardBattle = new DataEntities.CardBattle();

            using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var userID = HttpContext.Session.GetInt32("UserID");

                cardBattle = cardBattleBLL.CreateCardBattle(userID);
            }

            if (cardBattle == null || cardBattle == new DataEntities.CardBattle())
            {
                return View("~/Views/Home/Error.cshtml", new Models.Error() { Message = "Card battle couldn't be created" });
            }

            return View(cardBattle);
        }

        public IActionResult View(int ID)
        {
            var cardBattle = new DataEntities.CardBattle();

            using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                cardBattle = cardBattleBLL.GetCardBattle(ID);
            }

            return View(cardBattle);
        }

        public IActionResult CardBattleWinner(DataEntities.CardBattle.CardBattleWinner cardBattleWinner)
        {
            var success = false;

            cardBattleWinner.Message = "Success!";

            try
            {
                using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    var cardBattle = cardBattleBLL.GetCardBattle(cardBattleWinner.ID);

                    if (cardBattle == null)
                    {
                        cardBattleWinner.Message = "Can't find card battle";
                    }
                    else
                    {
                        if (cardBattle.Card1.WhiteCardID != cardBattleWinner.WinnerID && cardBattle.Card2.WhiteCardID != cardBattleWinner.WinnerID)
                        {
                            cardBattleWinner.Message = "Winner card isn't in this battle";
                        }
                        else
                        {
                            cardBattle.WinningCard = new DataEntities.WhiteCard()
                            {
                                WhiteCardID = cardBattleWinner.WinnerID
                            };

                            cardBattle.UserID = HttpContext.Session.GetInt32("UserID");

                            var saveResult = cardBattleBLL.SaveCardBattle(cardBattle);

                            if (saveResult != null)
                            {
                                success = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                cardBattleWinner.Message = e.Message;
            }

            cardBattleWinner.Success = success;

            return Json(cardBattleWinner);
        }
    }
}