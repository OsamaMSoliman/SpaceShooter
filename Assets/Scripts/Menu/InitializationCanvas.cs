using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class InitializationCanvas : MonoBehaviour
    {
        public void OnClickInitBtn() => AuthenticationManager.LoginAnonymously();

        public void OnEndEditPlayerName(string name) => LobbyManager.Instance.PlayerName = name.Trim();
    }
}
