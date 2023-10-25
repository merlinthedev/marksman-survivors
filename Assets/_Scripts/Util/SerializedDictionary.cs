using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Util {
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue> {
        [SerializeField] private List<MPair<TKey, TValue>> pairs = new();
    }

    [Serializable]
    public class MPair<TKey, TValue> {
        public TKey key;
        public TValue value;
    }
}