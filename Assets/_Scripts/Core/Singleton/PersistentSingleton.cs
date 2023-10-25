using UnityEngine;

namespace _Scripts.Core.Singleton {
    public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

    }
}