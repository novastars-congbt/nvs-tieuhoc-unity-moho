using System.Collections;
using UnityEngine;

namespace Novastars.MiniGame.DuaXe
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        protected static string ClassName = "";
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<T>();
                if (_instance == null) _instance = new GameObject($"SingletonBehaviour").AddComponent<T>();

                return _instance;
            }

            private set { }
        }

        protected void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
        }

        protected void OnDisable()
        {
            Debug.LogError("================ null roi");
            _instance = null;
        }
    }
}