using UnityEngine;

namespace _Scripts.Util {
    /**
     * A class that can be inherited from to toggle debug logging on and off in the inspector.
     */
    public class MonoLogger : MonoBehaviour {
        [SerializeField] protected bool shouldLog = true;

        protected void Log(object message) {
            Log(message, this);
        }

        protected void Log(object message, MonoBehaviour context) {
            if (shouldLog) Debug.Log(message, context);
        }
    }
}