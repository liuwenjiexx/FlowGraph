using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [FlowGraph]
    [Category(FlowNode.ActionsCategory + "/Wait")]
    internal class Wait
    {
        [Name("Delay")]
        [CoroutineMethod]
        public static IEnumerator Delay(int seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        [Name("WaitForSeconds")]
        [CoroutineMethod]
        public static IEnumerator WaitForSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
        [Name("WaitForSecondsRealtime")]
        [CoroutineMethod]
        public static IEnumerator WaitForSecondsRealtime(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
        }
        [Name("WaitForEndOfFrame")]
        [CoroutineMethod]
        public static IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
        }
        [Name("WaitForFixedUpdate")]
        [CoroutineMethod]
        public static IEnumerator WaitForFixedUpdate()
        {
            yield return new WaitForFixedUpdate();
        }
        //[CoroutineMethod]
        //public static IEnumerator WaitWhile(bool predicate)
        //{
        //    yield return new WaitWhile(() => predicate);
        //}
    }
}