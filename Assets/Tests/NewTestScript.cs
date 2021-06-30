using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test(Description = "Test 1")]
        public void NewTestScriptSimplePasses()
        {

        }
        [Test(Description = "Test fail")]
        public void TestFail()
        {
            Assert.Fail();
        }
        [Test(Description = "Test pass")]
        public void TestPass()
        {
            Assert.Pass();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
