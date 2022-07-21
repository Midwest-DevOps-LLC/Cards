using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Hubs
{
    public partial class ChatHub : Hub
    {
        public async Task SendMessage(string guid, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", guid, message);
        }

        public async Task Join(int userID, int gameID)
        {
            var newPlayer = new PlayerClient() { ConnectionID = Context.ConnectionId, GUID = System.Guid.NewGuid().ToString(), UserID = userID, GameID = gameID };

            var validationEvent = new DataEntities.ValidationEvent("", "");

            var userBLL = new BusinessLogicLayer.Users(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent);
            var user = userBLL.GetUserByUserID(userID, true);

            newPlayer.Username = user.Username;

            GameServer.AddPlayer(newPlayer);

            await Clients.Caller.SendAsync("YouJoined", newPlayer);

            await Clients.Others.SendAsync("PlayerJoined", newPlayer);
        }

        public async Task Quit(string guid)
        {
            var player = GameServer.Players.Where(x => x.GUID == guid).ToList();

            if (player.Count() > 0)
            {
                await Clients.Others.SendAsync("PlayerLeft", player.FirstOrDefault().GUID);

                GameServer.Players.RemoveAll(x => x.GUID == guid);
            }
        }

        public async Task GetPlayerList(int gameID)
        {
            var validationEvent = new DataEntities.ValidationEvent("", "");
            var userBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent);
            var gameState = userBLL.GetLatestGameStateByGameID(gameID);

            var czar = gameState.CurrentPlayerCzar;

            foreach (var p in GameServer.Players)
            {
                var player = gameState.Players.Where(x => x.UserID == p.UserID);

                if (player.Any())
                {
                    player.FirstOrDefault().GUID = p.GUID;

                    if (player.FirstOrDefault().UserID == czar.UserID)
                    {
                        player.FirstOrDefault().IsCzar = true;
                    }
                }
            }

            await Clients.Caller.SendAsync("PlayerList", gameState.Players);
        }

        public async Task SubmitCard(int userID, int cardID, int gameID)
        {
            var validationEvent = new DataEntities.ValidationEvent("", "");

            var userBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent);
            var whiteCardBLL = new BusinessLogicLayer.WhiteCard(userBLL.GetConnection());

            var card = whiteCardBLL.GetWhiteCardByID(cardID, true);
            var gameState = userBLL.GetLatestGameStateByGameID(gameID);

            gameState = userBLL.SubmitCard(new DataEntities.Player() { UserID = userID }, card, gameState);

            await Clients.All.SendAsync("SubmittedCard", userID); //If all players have submitted, pass in whitecards to send out to everyone

            var nonCzarPlayers = gameState.Players.Where(x => x.UserID != gameState.CurrentPlayerCzar.UserID);



            if (nonCzarPlayers.Any(x => x.SubmittedCards.Count() <= 0) == false) //All players have submitted
            {
                var allCards = new List<DataEntities.WhiteCard>();

                foreach (var player in nonCzarPlayers)
                {
                    allCards.AddRange(player.SubmittedCards);
                }

                gameState.ShowPlayedCards = true;
                userBLL.SaveGameState(gameState);

                await Clients.All.SendAsync("AllCardsSubmitted", allCards);
            }
        }

        public async Task SubmitCardCzar(int userID, int cardID, int gameID)
        {
            var validationEvent = new DataEntities.ValidationEvent("", "");

            var userBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent);
            var whiteCardBLL = new BusinessLogicLayer.WhiteCard(userBLL.GetConnection());

            var card = whiteCardBLL.GetWhiteCardByID(cardID, true);
            var gameState = userBLL.GetLatestGameStateByGameID(gameID);
            var game = userBLL.GetGameByGameID(gameID, true, true);

            var wonPlayer = new DataEntities.Player();

            foreach (var player in gameState.Players)
            {
                foreach (var submittedCard in player.SubmittedCards)
                {
                    if (submittedCard.WhiteCardID == cardID)
                    {
                        wonPlayer = player;

                        break;
                    }
                }
            }

            gameState = userBLL.SelectCardWinner(wonPlayer, gameState, game);



            await Clients.All.SendAsync("PlayerWonRound", wonPlayer.UserName, wonPlayer.UserID, gameState.CurrentPlayerCzar, gameState.CurrentBlackCard);
        }

        public async Task GetCards(int userID, int gameID)
        {
            var validationEvent = new DataEntities.ValidationEvent("", "");

            var userBLL = new BusinessLogicLayer.Game(MDO.Utility.Standard.ConnectionHandler.GetConnectionString(), ref validationEvent);

            var gameState = userBLL.GetLatestGameStateByGameID(gameID);

            var player = gameState.Players.Where(x => x.UserID == userID).FirstOrDefault();

            await Clients.Caller.SendAsync("CardsRecieved", player.WhiteCards);
        }
    }

    public static class GameServer
    {
        public static List<PlayerClient> Players { get; set; } = new List<PlayerClient>();

        public static void AddPlayer(PlayerClient player) //Check based on connection id
        {
            bool foundDuplicate = false;

            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ConnectionID == player.ConnectionID)
                {
                    Players[i] = player;

                    foundDuplicate = true;
                }
            }

            if (foundDuplicate == false)
            {
                Players.Add(player);
            }
        }
    }

    public class PlayerClient
    {
        public int UserID { get; set; }
        public int GameID { get; set; }
        public string Username { get; set; }
        public string GUID { get; set; }
        public string ConnectionID { get; set; }
    }
}
