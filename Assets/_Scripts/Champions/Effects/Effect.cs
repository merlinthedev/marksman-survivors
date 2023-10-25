using UnityEngine;

namespace Champions.Effects {
    public abstract class Effect : MonoBehaviour {
        protected Champion champion;

        public void Init(Champion champion) {
            this.champion = champion;
        }

        public abstract void OnApply();
        public abstract void OnExpire();

    }
}