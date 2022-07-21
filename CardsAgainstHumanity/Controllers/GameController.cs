using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstHumanity.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            var model = new Models.GameIndex();

            var validationEvents = new DataEntities.ValidationEvent("", "");

            using (BusinessLogicLayer.Game gameBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvents))
            {
                var publicGames = gameBLL.GetAllGames(false, true);

                model.PublicGames = publicGames;

                int? userID = HttpContext.Session.GetInt32("UserID");

                if (userID != null)
                {
                    var yourGames = gameBLL.GetGamesByUserID(userID.GetValueOrDefault(), false, null);

                    model.YourGames = yourGames;
                }
            }

            return View(model);
        }

        public IActionResult View(int? ID)
        {
            var gameEntity = new Models.GameView() { Game = new DataEntities.Game() };

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
                var validationEvent = new DataEntities.ValidationEvent("", "");

                using (var deckBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent))
                {
                    gameEntity = new Models.GameView(deckBLL.GetGameByGameID(ID.Value, true, true));

                    var gameOptionsBAL = new BusinessLogicLayer.GameOption(deckBLL.GetConnection(), ref validationEvent);

                    var gameOptions = gameOptionsBAL.GetAllGameOptions(true);

                    gameEntity.GameOptionsAvailable = gameOptions;
                }
            }

            return View(gameEntity);
        }

        public IActionResult Play(int? ID)
        {
            var gameEntity = new Models.GamePlay() { Game = new DataEntities.Game(), GameState = new DataEntities.GameState() };

            int? userID = HttpContext.Session.GetInt32("UserID");

            if (ID.HasValue == false)
            {
                return StatusCode(404);
            }
            else if (userID.HasValue)
            {
                var validationEvent = new DataEntities.ValidationEvent("", "");

                using (var deckBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent))
                {
                    gameEntity.Game = deckBLL.GetGameByGameID(ID.Value, true, true);

                    var player = new DataEntities.Player();
                    player.UserID = userID.Value;

                    var loadState = deckBLL.LoadGameState(gameEntity.Game);
                    gameEntity.GameState = loadState;

                    if (gameEntity.Game.CreatedBy != userID) //Is not the person who made the game
                    {
                        gameEntity.GameState = deckBLL.AddPlayer(gameEntity.GameState, player, gameEntity.Game);
                    }

                    if (userID == gameEntity.Game.CreatedBy && (loadState == null || loadState == new DataEntities.GameState()))
                    {
                        gameEntity.GameState = deckBLL.CreateGame(gameEntity.Game, player);
                    }
                    else
                    {
                        gameEntity.GameState = loadState;
                    }
                    
                }
            }
            else
            {
                return StatusCode(403);
            }

            return View(gameEntity);
        }

        public IActionResult Save(Models.GameSave cardBattleWinner)
        {
            var success = false;

            try
            {
                var validationEvent = new DataEntities.ValidationEvent("", "");

                using (var gameBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent))
                {
                    int? userID = HttpContext.Session.GetInt32("UserID");

                    if (userID.HasValue == false) //Not logged in
                    {
                        cardBattleWinner.Message = "You are not logged in";
                    }
                    else
                    {
                        var game = new DataEntities.Game();

                        if (cardBattleWinner.GameID.HasValue)
                        {
                            game = gameBLL.GetGameByGameID(cardBattleWinner.GameID.Value, true, null);
                        }

                        if (game == null || (game == new DataEntities.Game() && cardBattleWinner.GameID.HasValue))
                        {
                            cardBattleWinner.Message = "Couldn't find game";
                        }
                        else
                        {
                            if ((cardBattleWinner.GameID.HasValue && userID != game.CreatedBy) && userID != 1)
                            {
                                cardBattleWinner.Message = "You don't have access to save this game";
                            }
                            else
                            {
                                game.Name = cardBattleWinner.GameName;
                                game.CreatedBy = userID.Value;
                                game.ModifiedDate = DateTime.UtcNow;
                                game.CreatedDate = DateTime.UtcNow;
                                game.Active = true;

                                if (string.IsNullOrEmpty(cardBattleWinner.DecksIDs) == false)
                                {
                                    var st = cardBattleWinner.DecksIDs.Split(',').Select(x => new DataEntities.Deck() { DeckID = int.Parse(x) }).ToList();

                                    game.Decks = st;
                                }
                                else
                                {
                                    game.Decks = new List<DataEntities.Deck>();
                                }

                                if (string.IsNullOrEmpty(cardBattleWinner.OptionsIDs) == false)
                                {
                                    var st = cardBattleWinner.OptionsIDs.Split(',').Select(x => new DataEntities.GameOption() { GameOptionID = int.Parse(x), Active = true }).ToList();

                                    game.GameOptions = st;
                                }
                                else
                                {
                                    game.GameOptions = new List<DataEntities.GameOption>();
                                }

                                //Add current user to game
                                var user = new DataEntities.User();
                                user.UserID = userID.GetValueOrDefault();
                                user.Active = true;

                                game.Users.Add(user);

                                var gameSave = gameBLL.SaveGame(game);

                                if (gameSave.HasValue)
                                {
                                    cardBattleWinner.Message = "Success";
                                }
                                else
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
    }
}