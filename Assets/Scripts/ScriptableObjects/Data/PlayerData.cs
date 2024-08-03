using UnityEngine;


namespace Nsr.MultiSpaceShooter
{
    [CreateAssetMenu(menuName = "SOs/Data/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [field: SerializeField] private string playerName;
        public string PlayerName { get => playerName; set => playerName = value.Trim(); }
    }
}