using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using L33t.Network;
using L33t.UI;

namespace Tests
{
    public class WorldTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void WorldTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator WorldTestWithEnumeratorPasses()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
            yield return null;

            NetSystem NetSys = GameObject.FindObjectOfType<NetSystem>();
            Assert.NotNull(NetSys);
            PlayerClient PC = GameObject.FindObjectOfType<PlayerClient>();
            Assert.NotNull(PC);   
            MultiMenu MM = GameObject.FindObjectOfType<MultiMenu>();
            Assert.NotNull(MM);
            Assert.AreSame(NetSys,MM.NetSystem,"MultiMenu and the Scene have Diffrent NetSystems");
            yield return null;
            MM.HostGame();
            Color PlayerColor = Random.ColorHSV();
            Debug.Log($"Random Color Selected: {PlayerColor}");
            PC.CP.C = PlayerColor;
            yield return new WaitUntil(() => MM.NetSystem.isNetworkActive);
            Assert.AreEqual(PlayerColor, PC.NP.Color);
            Assert.AreEqual(PlayerColor, PC.PB.Body.GetComponent<SpriteRenderer>().color);
            yield return new WaitForSeconds(10);
            Assert.Less(PC.PB.Body.rb.velocity.magnitude,.01f,"The player was moving after one second with no interaction");
        }
    }
}
