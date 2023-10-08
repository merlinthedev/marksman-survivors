﻿using UnityEngine;

namespace Core.Singleton {
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T instance;

        protected virtual void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            }

            instance = this as T;
        }

        protected virtual void OnApplicationQuit() {
            instance = null;
            Destroy(gameObject);
        }

        public static T GetInstance() {
            return instance;
        }

    }
}