using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace _Scripts.Util {
    public class Overseer<TKey, TValue> : Dictionary<TKey, TValue> {
        public Overseer() { }

        public Overseer([NotNull] IDictionary<TKey, TValue> dictionary) : base(dictionary) {
            if (dictionary.Count > 1) throw new System.Exception("Overseer cannot have more than one entry");
        }

        public Overseer([NotNull] IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(
            dictionary, comparer) {
            if (dictionary.Count > 1) throw new System.Exception("Overseer cannot have more than one entry");
        }

        public Overseer(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) {
            if (collection.Count() > 1) throw new System.Exception("Overseer cannot have more than one entry");
        }

        public Overseer(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) :
            base(collection, comparer) {
            throw new NotImplementedException("Overseer cannot have more than one entry");
        }

        public Overseer(IEqualityComparer<TKey> comparer) : base(comparer) {
            throw new NotImplementedException("Constructor not implemented");
        }

        public Overseer(int capacity) : base(capacity) {
            if (capacity > 1) throw new System.Exception("Overseer cannot have more than one entry");
        }

        public Overseer(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) {
            if (capacity > 1) throw new System.Exception("Overseer cannot have more than one entry");
        }

        protected Overseer(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public void Add(TKey k, TValue v) {
            if (this.Count == 1) {
                Console.WriteLine("Overseer already has an entry, returning...");
                return;
            }

            base.Add(k, v);
        }

        public new TValue this[TKey key] => base[key];

        public override string ToString() {
            return this.Aggregate("", (current, entry) => current + (entry.Key + " : " + entry.Value + "\n"));
        }
    }
}