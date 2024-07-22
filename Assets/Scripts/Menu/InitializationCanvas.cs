using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class InitializationCanvas : MonoBehaviour
    {
        [SerializeField] private AuthenticationManagerSO authenticationManager;

        public void OnClickInitBtn() => authenticationManager.LoginAnonymously();

        public void OnEndEditPlayerName(string name) => LobbyManager.Instance.PlayerName = name.Trim();
    }
}
