using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using L33t.Network;

namespace Tests
{
    public class NetTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NetTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NetTestWithEnumeratorPasses()
        {
            Debug.Log(Time.frameCount);
            //Net
            yield return null;
        }
    }
}
