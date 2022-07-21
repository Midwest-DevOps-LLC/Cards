using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstHumanity.Controllers
{
    public class DeckController : Controller
    {
        //todo deck viewer
        //deck creator with card creator inside

        public IActionResult Index()
        {
            var index = new Models.DeckIndex();

            using (var deckBLL = new BusinessLogicLayer.Deck(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                int? userID = HttpContext.Session.GetInt32("UserID");

                var decks = deckBLL.GetAllDecks(false);

                var systemDecks = decks.Where(x => x.CreatedBy == 0).ToList();
                var userDecks = decks.Where(x => x.CreatedBy == userID).ToList();

                index.Decks = systemDecks;
                index.CustomDecks = userDecks;
            }

            return View(index);
        }

        public IActionResult View(int? ID)
        {
            var cardBattle = new Models.DeckView() { Deck = new DataEntities.Deck() };

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
                using (var deckBLL = new BusinessLogicLayer.Deck(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    cardBattle = new Models.DeckView(deckBLL.GetDeckByID(ID.Value, true));
                }
            }

            return View(cardBattle);
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

        public IActionResult BlackCardList(string filterName, string filterDeck)
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

            return PartialView("_BlackCardAdd", cardBattle);
        }

        public IActionResult DeckList(string filterName, string filterDeck)
        {
            var cardBattle = new List<DataEntities.Deck>();

            int? userID = HttpContext.Session.GetInt32("UserID");

            using (var deckBLL = new BusinessLogicLayer.Deck(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var allCards = deckBLL.GetAllDecks(false);

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
                    cardBattle = allCards.Where(x => x.Name.ToUpper().Contains(filterName.ToUpper())).ToList();
                }
            }

            return PartialView("_DeckAdd", cardBattle);
        }

        public IActionResult Save(Models.DeckSave cardBattleWinner)
        {
            var success = false;

            try
            {
                using (var deckBLL = new BusinessLogicLayer.Deck(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                {
                    int? userID = HttpContext.Session.GetInt32("UserID");

                    if (userID.HasValue == false) //Not logged in
                    {
                        cardBattleWinner.Message = "You are not logged in";
                    }
                    else
                    {
                        var deck = new DataEntities.Deck();

                        if (cardBattleWinner.DeckID.HasValue)
                        {
                            deck = deckBLL.GetDeckByID(cardBattleWinner.DeckID.Value, false);
                        }

                        if (deck == null || (deck == new DataEntities.Deck() && cardBattleWinner.DeckID.HasValue))
                        {
                            cardBattleWinner.Message = "Couldn't find deck";
                        }
                        else
                        {
                            if ((cardBattleWinner.DeckID.HasValue && userID != deck.CreatedBy) && userID != 1)
                            {
                                cardBattleWinner.Message = "You don't have access to save this deck";
                            }
                            else
                            {
                                deck.Name = cardBattleWinner.DeckName;
                                deck.Description = cardBattleWinner.DeckName;
                                deck.CreatedBy = userID.Value;

                                if (string.IsNullOrEmpty(cardBattleWinner.WhiteCardIDs) == false)
                                {
                                    var st = cardBattleWinner.WhiteCardIDs.Split(',').Select(x => new DataEntities.WhiteCard() { WhiteCardID = int.Parse(x) }).ToList();

                                    deck.WhiteCards = st;
                                }

                                if (string.IsNullOrEmpty(cardBattleWinner.BlackCardIDs) == false)
                                {
                                    var st = cardBattleWinner.BlackCardIDs.Split(',').Select(x => new DataEntities.BlackCard() { BlackCardID = int.Parse(x) }).ToList();

                                    deck.BlackCards = st;
                                }

                                var deckSave = deckBLL.SaveCardBattle(deck, true, true);

                                if (deckSave.HasValue)
                                {
                                    cardBattleWinner.Message = "Success";
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                cardBattleWinner.Message = e.Message;
            }

            //cardBattleWinner.Message = "Success!";

            //try
            //{
            //    using (var cardBattleBLL = new BusinessLogicLayer.CardBattle(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            //    {
            //        var cardBattle = cardBattleBLL.GetCardBattle(cardBattleWinner.ID);

            //        if (cardBattle == null)
            //        {
            //            cardBattleWinner.Message = "Can't find card battle";
            //        }
            //        else
            //        {
            //            if (cardBattle.Card1.WhiteCardID != cardBattleWinner.WinnerID && cardBattle.Card2.WhiteCardID != cardBattleWinner.WinnerID)
            //            {
            //                cardBattleWinner.Message = "Winner card isn't in this battle";
            //            }
            //            else
            //            {
            //                cardBattle.WinningCard = new DataEntities.WhiteCard()
            //                {
            //                    WhiteCardID = cardBattleWinner.WinnerID
            //                };

            //                cardBattle.UserID = HttpContext.Session.GetInt32("UserID");

            //                var saveResult = cardBattleBLL.SaveCardBattle(cardBattle);

            //                if (saveResult != null)
            //                {
            //                    success = true;
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    cardBattleWinner.Message = e.Message;
            //}

            //cardBattleWinner.Success = success;

            return Json(cardBattleWinner);
        }
    }
}