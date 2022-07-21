using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstHumanity.Controllers
{
    public class WhiteCardController : Controller
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
            var cardBattle = new Models.WhiteCardSaveModel();

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
                using (var deckBLL = new BusinessLogicLayer.WhiteCard(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    cardBattle = new Models.WhiteCardSaveModel(deckBLL.GetWhiteCardByID(ID.Value, false));
                }
            }

            return View(cardBattle);
        }

        public IActionResult Save(Models.WhiteCardSaveModel cardBattleWinner)
        {
            var success = false;

            try
            {
                using (var whiteCardBLL = new BusinessLogicLayer.WhiteCard(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    int? userID = HttpContext.Session.GetInt32("UserID");

                    if (userID.HasValue == false) //Not logged in
                    {
                        cardBattleWinner.Message = "You are not logged in";
                    }
                    else
                    {
                        var deck = new DataEntities.WhiteCard();

                        if (cardBattleWinner.WhiteCardID.HasValue)
                        {
                            deck = whiteCardBLL.GetWhiteCardByID(cardBattleWinner.WhiteCardID.Value, false);
                        }

                        if (deck == null || (deck == new DataEntities.WhiteCard() && cardBattleWinner.WhiteCardID.HasValue))
                        {
                            cardBattleWinner.Message = "Couldn't find white card";
                        }
                        else
                        {
                            if ((cardBattleWinner.WhiteCardID.HasValue && userID != deck.CreatedBy) && userID != 1)
                            {
                                cardBattleWinner.Message = "You don't have access to save this white card";
                            }
                            else
                            {
                                deck.Text = cardBattleWinner.Text;
                                deck.CreatedBy = userID.Value;

                                var deckSave = whiteCardBLL.SaveWhiteCard(deck);

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
            var cardBattle = new List<DataEntities.WhiteCard>();

            int? userID = HttpContext.Session.GetInt32("UserID");

            using (var deckBLL = new BusinessLogicLayer.WhiteCard(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var allCards = deckBLL.GetAllWhiteCards(false);

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

        //public IActionResult View(int ID)
        //{
        //    var cardBattle = new DataEntities.CardBattle();

        //    using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
        //    {
        //        cardBattle = cardBattleBLL.GetCardBattle(ID);
        //    }

        //    return View(cardBattle);
        //}

        //public IActionResult CardBattleWinner(DataEntities.CardBattle.CardBattleWinner cardBattleWinner)
        //{
        //    var success = false;

        //    cardBattleWinner.Message = "Success!";

        //    try
        //    {
        //        using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
        //        {
        //            var cardBattle = cardBattleBLL.GetCardBattle(cardBattleWinner.ID);

        //            if (cardBattle == null)
        //            {
        //                cardBattleWinner.Message = "Can't find card battle";
        //            }
        //            else
        //            {
        //                if (cardBattle.Card1.WhiteCardID != cardBattleWinner.WinnerID && cardBattle.Card2.WhiteCardID != cardBattleWinner.WinnerID)
        //                {
        //                    cardBattleWinner.Message = "Winner card isn't in this battle";
        //                }
        //                else
        //                {
        //                    cardBattle.WinningCard = new DataEntities.WhiteCard()
        //                    {
        //                        WhiteCardID = cardBattleWinner.WinnerID
        //                    };

        //                    cardBattle.UserID = HttpContext.Session.GetInt32("UserID");

        //                    var saveResult = cardBattleBLL.SaveCardBattle(cardBattle);

        //                    if (saveResult != null)
        //                    {
        //                        success = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        cardBattleWinner.Message = e.Message;
        //    }

        //    cardBattleWinner.Success = success;

        //    return Json(cardBattleWinner);
        //}
    }
}