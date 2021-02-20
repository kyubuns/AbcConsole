using UnityEngine;

namespace AbcConsole
{
    public class Root : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}