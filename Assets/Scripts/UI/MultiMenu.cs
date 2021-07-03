using L33t.Network;
using Mirror;
using UnityEngine.UI;

namespace L33t.UI
{
    public class MultiMenu : Menu
    {
        public NetSystem NetSystem;

        public InputField ServerIP;
        public Text Displaytext;
        bool isJoin = false;
        private void Update()
        {
            if (NetworkClient.isConnected && isJoin)
            {
                isJoin = false;
                Close();
            }
            if (isJoin && !NetworkClient.active)
            {
                Displaytext.text = $"Connection to :{NetSystem.networkAddress} Failed :(";
                isJoin = false;
            }
        }
        public void HostGame()
        {
            NetSystem.StartHost();
            Close();
        }
        public void JoinGame()
        {
            NetSystem.networkAddress = ServerIP.text;
            NetSystem.StartClient();
            Displaytext.text = $"Connecting to :" + NetSystem.networkAddress;
            isJoin = true;
        }
    }
}