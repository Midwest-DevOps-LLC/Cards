using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstHumanity.Controllers
{
    public class BlackCardController : Controller
    {
        //todo deck viewer
        //deck creator with card creator inside

        public IActionResult Index()
        {
            //var index = new DataEntities.CardBattle.CardBattleIndex();

            //using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            //{
            //    var rankings = cardBattleBLL.GetAllCardBattleRanking();

            //    index.WhiteCards = rankings;
            //}

            return View();
        }

        public IActionResult View(int? ID)
        {
            var cardBattle = new Models.BlackCardSaveModel();

            if (ID.HasValue == false)
            {
                int? userID = HttpContext.Session.GetInt32("UserID");

                if (userID.HasValue == false)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                using (var deckBLL = new BusinessLogicLayer.BlackCard(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    cardBattle = new Models.BlackCardSaveModel(deckBLL.GetBlackCardByID(ID.Value, false));
                }
            }

            return View(cardBattle);
        }

        public IActionResult Save(Models.BlackCardSaveModel cardBattleWinner)
        {
            var success = false;

            try
            {
                using (var whiteCardBLL = new BusinessLogicLayer.BlackCard(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    int? userID = HttpContext.Session.GetInt32("UserID");

                    if (userID.HasValue == false) //Not logged in
                    {
                        cardBattleWinner.Message = "You are not logged in";
                    }
                    else
                    {
                        var deck = new DataEntities.BlackCard();

                        if (cardBattleWinner.BlackCardID.HasValue)
                        {
                            deck = whiteCardBLL.GetBlackCardByID(cardBattleWinner.BlackCardID.Value, false);
                        }

                        if (deck == null || (deck == new DataEntities.BlackCard() && cardBattleWinner.BlackCardID.HasValue))
                        {
                            cardBattleWinner.Message = "Couldn't find black card";
                        }
                        else
                        {
                            if ((cardBattleWinner.BlackCardID.HasValue && userID != deck.CreatedBy) && userID != 1)
                            {
                                cardBattleWinner.Message = "You don't have access to save this black card";
                            }
                            else
                            {
                                deck.Text = cardBattleWinner.Text;
                                deck.Pick = cardBattleWinner.Pick;
                                deck.Draw = cardBattleWinner.Draw;
                                deck.CreatedBy = userID.Value;

                                var deckSave = whiteCardBLL.SaveBlackCard(deck);

                                if (deckSave.HasValue)
                                {
                                    cardBattleWinner.Message = "Success";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                cardBattleWinner.Message = e.Message;
            }

            return Json(cardBattleWinner);
        }

        public IActionResult CardList(string filterName, string filterDeck)
        {
            var cardBattle = new List<DataEntities.BlackCard>();

            int? userID = HttpContext.Session.GetInt32("UserID");

            using (var deckBLL = new BusinessLogicLayer.BlackCard(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var allCards = deckBLL.GetAllBlackCards(false);

                if (filterDeck == "s1" && userID.HasValue)
                {
                    allCards = allCards.Where(x => x.CreatedBy == 0 || x.CreatedBy == userID.Value).ToList();
                }
                else if (filterDeck == "s2")
                {
                    allCards = allCards.Where(x => x.CreatedBy == 0).ToList();
                }
                else if (filterDeck == "s3" && userID.HasValue)
                {
                    allCards = allCards.Where(x => x.CreatedBy == userID.Value).ToList();
                }

                if (string.IsNullOrEmpty(filterName))
                {
                    cardBattle = allCards;
                }
                else
                {
                    cardBattle = allCards.Where(x => x.Text.ToUpper().Contains(filterName.ToUpper())).ToList();
                }
            }

            return PartialView("_CardAdd", cardBattle);
        }
    }
}