using System.Linq;
using Unity.Services.Lobbies.Models;

namespace Nsr.MultiSpaceShooter
{
    public static class LobbyExtentions
    {
        public static Player GetHost(this Lobby lobby)
        {
            return lobby.Players.First<Player>(p => p.Id == lobby.HostId);
        }
        public static string GetHostName(this Lobby lobby)
        {
            lobby.GetHost().Data.TryGetValue(Constants.PLAYER_NAME, out var playerName);
            return playerName.Value;;
        }
    }
}
