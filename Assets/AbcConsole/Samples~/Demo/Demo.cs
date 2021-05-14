using UnityEngine;

namespace AbcConsole.Demo
{
    public class Demo : MonoBehaviour
    {
        public void Start()
        {
            for (var i = 0; i < 50; ++i)
            {
                Debug.Log($"Test {i}");
            }
        }
    }

}