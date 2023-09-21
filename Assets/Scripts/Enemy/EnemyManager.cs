using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Enemy {
    public class EnemyManager : MonoBehaviour {
        [SerializeField] private Player player; // THIS IS BAD LETS NOT DO THIS
        [SerializeField] private Enemy enemyPrefab;

        // exposed to the editor because there were some unexplainable things happening :D
        [SerializeField] private bool shouldSpawn = false;
        [SerializeField] private int maxAmountOfEnemies = 1;

        [SerializeField] [Tooltip("Time it takes between enemy spawn")]
        private float spawnTimer = 1.4f;


        private int amountOfEnemies = 0;
        private static EnemyManager instance;
        private Dictionary<Collider, Enemy> enemyDictionary = new();

        private void OnEnable() {
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);
            EventBus<UILevelUpPanelOpenEvent>.Subscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Subscribe(OnLevelUpPanelClosed);
        }

        private void OnDisable() {
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);
            EventBus<UILevelUpPanelOpenEvent>.Unsubscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Unsubscribe(OnLevelUpPanelClosed);
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
            while (shouldSpawn) {
                if (amountOfEnemies >= maxAmountOfEnemies) {
                    shouldSpawn = false;
                    break;
                }

                yield return new WaitForSeconds(spawnTimer);
                // Logger.Log("Spawing enemy", Logger.Color.BLUE, this);
                Enemy enemy = Instantiate(enemyPrefab, FindPositionIteratively(), Quaternion.Euler(0, 45, 0));
                enemy.SetTarget(player.transform);
                //enemy.SetCanMove(false); // For when we want to test stuff on enemies that should not move
                enemyDictionary.Add(enemy.GetComponent<Collider>(), enemy);
                amountOfEnemies++; // For when we want to limit the amount of enemies on the screen for testing purposes
            }
        }

        private Vector3 FindPositionRecursively() {
            Vector3 randomPointOnPlane = new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f));


            // Take the player position, shoot two rays from this position, one 45 deg to the right of the players
            // direction, one 45 deg to the left
            Vector3 playerPos = player.gameObject.transform.position;
            Vector3 playerDirection = player.GetCurrentlySelectedChampion().GetCurrentMovementDirection();

            // get the direction into where we should find our triangle points
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerDirection;
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerDirection;

            // our triangle points are 30 units away from the player
            Vector3 rightPoint = playerPos + rightDirection * 30f;
            Vector3 leftPoint = playerPos + leftDirection * 30f;

            // debug lines to visualize the triangle
            Debug.DrawLine(playerPos, rightPoint, Color.yellow, 0.5f);
            Debug.DrawLine(playerPos, leftPoint, Color.yellow, 0.5f);
            Debug.DrawLine(leftPoint, rightPoint, Color.yellow, 0.5f);

            // check if the random point is inside the no spawn triangle
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
                // find a random point on our playing field
                randomPointOnPlane = new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f));

                // get the player position and the current direction the player is moving in
                Vector3 playerPos = player.gameObject.transform.position;
                Vector3 playerDirection = player.GetCurrentlySelectedChampion().GetCurrentMovementDirection();

                // get two directions 45 degrees to the left and right of the player direction
                Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerDirection;
                Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerDirection;

                // get two points in the previous directions and 30 units away from the player
                Vector3 rightPoint = playerPos + rightDirection * 30f;
                Vector3 leftPoint = playerPos + leftDirection * 30f;

                // draw the triangle for debug purposes
                Debug.DrawLine(playerPos, rightPoint, Color.yellow, 0.5f);
                Debug.DrawLine(playerPos, leftPoint, Color.yellow, 0.5f);
                Debug.DrawLine(leftPoint, rightPoint, Color.yellow, 0.5f);

                // check if the random point is inside the no-spawn triangle
                bool isInTriangle = Utilities.IsInsideTriangle(
                    new Vector2(playerPos.x, playerPos.z),
                    new Vector2(rightPoint.x, rightPoint.z),
                    new Vector2(leftPoint.x, leftPoint.z),
                    new Vector2(randomPointOnPlane.x, randomPointOnPlane.z));

                // if it is not we can break out of our loop and return the point, it is now a valid spawn position
                if (!isInTriangle) {
                    break;
                }
            } while (true);

            return randomPointOnPlane;
        }

        private void OnEnemyKilledEvent(EnemyKilledEvent enemyKilledEvent) {
            enemyDictionary.Remove(enemyKilledEvent.m_Collider);
        }

        private void OnLevelUpPanelOpen(UILevelUpPanelOpenEvent e) {
            shouldSpawn = false;
            foreach (var kvp in enemyDictionary) {
                kvp.Value.OnPause();
            }
        }

        private void OnLevelUpPanelClosed(UILevelUpPanelClosedEvent e) {
            shouldSpawn = true;
            foreach (var kvp in enemyDictionary) {
                kvp.Value.OnResume();
            }
        }

        public Enemy GetClosestEnemy(Vector3 position, List<Enemy> enemiesToIgnore = null) {
            float closestDistance = Mathf.Infinity;

            Enemy closestEnemy = null;
            foreach (var enemy in enemyDictionary) {
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
            return (from enemy in enemyDictionary
                where Vector3.Distance(point, enemy.Value.transform.position) < radius
                select enemy.Value).ToList();
        }

        public Enemy GetEnemy(Collider other) {
            Enemy enemy = enemyDictionary[other];
            if (enemy == null) {
                Util.Logger.Log("Enemy not found in dictionary", Logger.Color.RED, this);
                throw new Exception("Enemy not found in dictionary");
            }

            return enemy;
        }

        public void SetShouldSpawn(bool value) {
            shouldSpawn = value;

            if (value) {
                StartCoroutine(SpawnEnemy());
            }
            else {
                StopCoroutine(SpawnEnemy());
            }
        }

        public static EnemyManager GetInstance() {
            return instance;
        }
    }
}