using _Scripts.Champions;
using _Scripts.Core;
using _Scripts.Util;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Entities
{
    public class StickyBomb : MonoBehaviour, IAttachable
    {
        private IDamager source;
        private IDamageable damageable;
        private float timeToExplode;
        private float radius;
        private float damagePercentage;

        private Coroutine cancelable;

        public void Init(float timeToExplode, float radius, float damagePercentage)
        {
            this.timeToExplode = timeToExplode;
            this.radius = radius;
            this.damagePercentage = damagePercentage;
        }

        public void OnAttach(IDamageable damageable, IDamager source)
        {
            this.damageable = damageable;
            this.source = source;
            this.damageable.attachables.Add(this);

            cancelable = Utilities.InvokeDelayed(OnUse, timeToExplode, this);
        }

        public void OnUse()
        {
            StopCoroutine(cancelable);
            cancelable = null;
            // StopAllCoroutines(); // cancel for if we call this function before the timeToExplode is up.
            List<IDamageable> damageables = DamageableManager.GetInstance()
                .GetDamageablesInArea(damageable.GetTransform().position, radius);
            foreach (var dam in damageables)
            {
                source.DealDamage(dam, (float)(source as Champion)?.GetAttackDamage() * damagePercentage,
                    Champion.DamageType.BASIC);
            }

            damageable.attachables.Remove(this);

            Destroy(gameObject);
        }
    }
}