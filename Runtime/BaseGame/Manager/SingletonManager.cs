using UnityEngine;

namespace Dma.BaseGame
{
    public abstract class SingletonManager : MonoBehaviour
    {
    }

    public abstract class SingletoManagern<T> : SingletonManager
        where T : SingletonManager
    {
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = GetInstance();
                }
                return s_Instance;
            }
        }

        private static T GetInstance()
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindObjectOfType<T>();
                Debug.LogWarningFormat($"FindObjectOfType had to be used to find the singleton {typeof(T)}");
            }

            return s_Instance;
        }

        protected virtual void Awake()
        {
            s_Instance = this as T;
        }
    }
}