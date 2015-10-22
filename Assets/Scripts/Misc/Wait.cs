using Singleton;
using System.Collections;
using UnityEngine;

namespace Misc
{
    public class Wait : MonoBehaviour
    {
        public IEnumerator WaitForFrames(int frames)
        {
            yield return StartCoroutine(HelperSingleton.Instance.WaitForFrames(frames));
        }

        public IEnumerator WaitForSeconds(int seconds)
        {
            yield return StartCoroutine(HelperSingleton.Instance.WaitForSeconds(seconds));
        }
    }
}
