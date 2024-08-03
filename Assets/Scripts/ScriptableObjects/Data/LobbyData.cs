using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    [CreateAssetMenu(menuName = "SOs/Data/LobbyData")]
    public class LobbyData : ScriptableObject
    {
        [SerializeField] private string lobbyName;
        public string LobbyName { get => lobbyName; set => lobbyName = value.Trim(); }
        [field: SerializeField] public int MaxPlayers { get; set; } = 2;
    }
}