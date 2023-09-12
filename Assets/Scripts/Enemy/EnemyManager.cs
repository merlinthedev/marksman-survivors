using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using UnityEngine;
using Util;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Enemy {
    public class EnemyManager : MonoBehaviour {
        [SerializeField]
        [Tooltip("Time it takes between enemy spawn")]
        private float m_SpawnTimer = 1.4f;

        private bool m_ShouldSpawn = false;
        [SerializeField] private int m_MaxAmountOfEnemies = 1;
        private int m_AmountOfEnemies = 0;
        [SerializeField] private Enemy m_EnemyPrefab;
        [SerializeField] private Player m_Player; // THIS IS BAD LETS NOT DO THIS

        private static EnemyManager instance;
        private Dictionary<Collider, Enemy> m_EnemyDictionary = new Dictionary<Collider, Enemy>();

        private void OnEnable() {
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);
        }

        private void OnDisable() {
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);
        }

        private void Start() {
            if (instance != null) {
                Destroy(this);
                return;
            }

            instance = this;

            StartCoroutine(SpawnEnemy());
        }

        private IEnumerator SpawnEnemy() {
            while (m_ShouldSpawn) {
                if (m_AmountOfEnemies >= m_MaxAmountOfEnemies) {
                    m_ShouldSpawn = false;
                    break;
                }
                yield return new WaitForSeconds(m_SpawnTimer);
                // Logger.Log("Spawing enemy", Logger.Color.BLUE, this);
                Enemy enemy = Instantiate(m_EnemyPrefab, FindPositionIteratively(), Quaternion.Euler(0, 45, 0));
                enemy.SetTarget(m_Player.transform);
                //enemy.SetCanMove(false); // For when we want to test stuff on enemies that should not move
                m_EnemyDictionary.Add(enemy.GetComponent<Collider>(), enemy);
                m_AmountOfEnemies++; // For when we want to limit the amount of enemies on the screen for testing purposes
            }
        }

        private Vector3 FindPositionRecursively() {
            Vector3 randomPointOnPlane = new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f));


            // Take the player position, shoot two rays from this position, one 45 deg to the right of the players direction, one 45 deg to the left
            Vector3 playerPos = m_Player.gameObject.transform.position;
            Vector3 playerDirection = m_Player.GetCurrentlySelectedChampion().GetCurrentMovementDirection();


            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerDirection;
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerDirection;

            Vector3 rightPoint = playerPos + rightDirection * 30f;
            Vector3 leftPoint = playerPos + leftDirection * 30f;

            Debug.DrawLine(playerPos, rightPoint, Color.yellow, 0.5f);
            Debug.DrawLine(playerPos, leftPoint, Color.yellow, 0.5f);
            Debug.DrawLine(leftPoint, rightPoint, Color.yellow, 0.5f);

            bool isInTriangle = Utilities.IsInsideTriangle(
                new Vector2(playerPos.x, playerPos.z)
                , new Vector2(rightPoint.x, rightPoint.z)
                , new Vector2(leftPoint.x, leftPoint.z)
                , new Vector2(randomPointOnPlane.x, randomPointOnPlane.z));


            // if the point is in the triangle, we want to try again
            if (isInTriangle) {
                return FindPositionRecursively();
            }


            // If the random point is in the triangle, return it
            return randomPointOnPlane;
        }

        private Vector3 FindPositionIteratively() {
            Vector3 randomPointOnPlane;

            do {
                randomPointOnPlane = new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f));

                Vector3 playerPos = m_Player.gameObject.transform.position;
                Vector3 playerDirection = m_Player.GetCurrentlySelectedChampion().GetCurrentMovementDirection();

                Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerDirection;
                Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerDirection;

                Vector3 rightPoint = playerPos + rightDirection * 30f;
                Vector3 leftPoint = playerPos + leftDirection * 30f;

                Debug.DrawLine(playerPos, rightPoint, Color.yellow, 0.5f);
                Debug.DrawLine(playerPos, leftPoint, Color.yellow, 0.5f);
                Debug.DrawLine(leftPoint, rightPoint, Color.yellow, 0.5f);

                bool isInTriangle = Utilities.IsInsideTriangle(
                    new Vector2(playerPos.x, playerPos.z),
                    new Vector2(rightPoint.x, rightPoint.z),
                    new Vector2(leftPoint.x, leftPoint.z),
                    new Vector2(randomPointOnPlane.x, randomPointOnPlane.z));

                if (!isInTriangle) {
                    // If the random point is not in the triangle, exit the loop
                    break;
                }
            } while (true);

            return randomPointOnPlane;
        }

        private void OnEnemyKilledEvent(EnemyKilledEvent enemyKilledEvent) {
            m_EnemyDictionary.Remove(enemyKilledEvent.m_Collider);
        }

        public Enemy GetClosestEnemy(Vector3 position, List<Enemy> enemiesToIgnore = null) {
            float closestDistance = Mathf.Infinity;

            Enemy closestEnemy = null;
            foreach (var enemy in m_EnemyDictionary) {
                // if (enemy.Value == enemyToIgnore) continue;
                if (enemiesToIgnore != null && enemiesToIgnore.Contains(enemy.Value)) continue;
                float distance = Vector3.Distance(position, enemy.Value.transform.position);
                if (!(distance < closestDistance)) continue;
                closestDistance = distance;
                closestEnemy = enemy.Value;
            }

            return closestEnemy;
        }

        public List<Enemy> GetEnemiesInArea(Vector3 point, float radius = 10f) {
            return (from enemy in m_EnemyDictionary
                where Vector3.Distance(point, enemy.Value.transform.position) < radius
                select enemy.Value).ToList();
        }

        public Enemy GetEnemy(Collider other) {
            Enemy enemy = m_EnemyDictionary[other];
            if (enemy == null) {
                Util.Logger.Log("Enemy not found in dictionary", Logger.Color.RED, this);
                throw new Exception("Enemy not found in dictionary");
            }

            return enemy;
        }

        public void SetShouldSpawn(bool value) {
            m_ShouldSpawn = value;

            if (value) {
                StartCoroutine(SpawnEnemy());
            } else {
                StopCoroutine(SpawnEnemy());
            }
        }

        public static EnemyManager GetInstance() {
            return instance;
        }
    }
}