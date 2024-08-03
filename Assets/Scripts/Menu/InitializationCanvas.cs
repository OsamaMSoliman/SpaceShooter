using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class InitializationCanvas : MonoBehaviour
    {
        // TODO: make it like the name of the lobby when it is empty it should show error and not allow to proceed
        public void OnClickInitBtn() => AuthenticationManager.LoginAnonymously();
    }
}
