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
        [SerializeField] private Player player; // THIS IS BAD LETS NOT DO THIS
        [SerializeField] private Enemy enemyPrefab;

        // exposed to the editor because there were some unexplainable things happening :D
        [SerializeField] private bool shouldSpawn = false;
        [SerializeField] private int maxAmountOfEnemies = 1;

        [SerializeField] [Tooltip("Time it takes between enemy spawn")]
        private float spawnTimer = 1.4f;

        private float lastSpawnTime = 0f;
        private float internalSpawnTimer;


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
            internalSpawnTimer = Random.Range(spawnTimer - 0.5f, spawnTimer + 0.5f);

            // StartCoroutine(SpawnEnemy());
        }

        private void Update() {
            // when i hold left shift, i want to log the mouse position on the camera viewport
            if (Input.GetKey(KeyCode.LeftShift)) {
                Vector3 mousePos = Input.mousePosition;
                Vector3 mousePosOnScreen = Camera.main.ScreenToViewportPoint(mousePos);
                Logger.Log("Mouse position on screen: " + mousePosOnScreen, Logger.Color.BLUE, this);
            }

            HandleEnemySpawn();
        }

        private void HandleEnemySpawn() {
            if (!shouldSpawn) return;

            // if the time since the last spawn is less than the spawn timer, we don't want to spawn
            if (Time.time - lastSpawnTime < spawnTimer) return;

            if (amountOfEnemies >= maxAmountOfEnemies) {
                shouldSpawn = false;
                return;
            }

            // find a random position on the playing field for our group of enemies

            // get a random int between 2 and 5
            int randomInt = Random.Range(3, 7);

            Vector3[] location = FindPositionsIteratively(randomInt);

            for (var i = 0; i < location.Length; i++) {
                Vector3 randomSpread = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                Enemy enemy = Instantiate(enemyPrefab, location[i] + randomSpread, Quaternion.Euler(0, 45, 0));
                enemy.SetTarget(player.transform);
                // enemy.SetCanMove(false); // For when we want to test stuff on enemies that should not move
                enemyDictionary.Add(enemy.GetComponent<Collider>(), enemy);
            }


            lastSpawnTime = Time.time;
            internalSpawnTimer = Random.Range(spawnTimer - 0.5f, spawnTimer + 0.5f);
        }

        [Obsolete("Deprecated, spawning is now handled by HandleEnemySpawn()")]
        private IEnumerator SpawnEnemy() {
            while (shouldSpawn) {
                if (amountOfEnemies >= maxAmountOfEnemies) {
                    shouldSpawn = false;
                    break;
                }

                yield return new WaitForSeconds(spawnTimer);
                // Logger.Log("Spawing enemy", Logger.Color.BLUE, this);
                Enemy enemy = Instantiate(enemyPrefab, FindPositionsIteratively()[0], Quaternion.Euler(0, 45, 0));
                enemy.SetTarget(player.transform);
                //enemy.SetCanMove(false); // For when we want to test stuff on enemies that should not move
                enemyDictionary.Add(enemy.GetComponent<Collider>(), enemy);
                amountOfEnemies++; // For when we want to limit the amount of enemies on the screen for testing purposes
            }
        }

        [Obsolete("This method is not used anymore, but it is kept for reference")]
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

        private Vector3[] FindPositionsIteratively(int amountOfIterations = 1) {
            /*
            bool isInTriangle = true;

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
                isInTriangle = Utilities.IsInsideTriangle(
                    new Vector2(playerPos.x, playerPos.z),
                    new Vector2(rightPoint.x, rightPoint.z),
                    new Vector2(leftPoint.x, leftPoint.z),
                    new Vector2(randomPointOnPlane.x, randomPointOnPlane.z));

                // if it is not we can break out of our loop and return the point, it is now a valid spawn position
            } while (isInTriangle);

             */

            // where in the camera viewport is the player
            Vector3 playerViewportPosition = Camera.main.WorldToViewportPoint(player.transform.position);

            // Logger.Log("Player viewport position: " + playerViewportPosition, Logger.Color.BLUE, this);

            // get a random point on the screen
            Vector3[] randomPointOutsideScreen = GenerateSpawnPoints(amountOfIterations);
            Debug.Log("Random point on screen: " + randomPointOutsideScreen);

            // get the world position of the random point on the screen
            // Vector3 randomPointOnPlane = Camera.main.ViewportToWorldPoint(randomPointOutsideScreen);

            for (int i = 0; i < randomPointOutsideScreen.Length; i++) {
                randomPointOutsideScreen[i] = Camera.main.ViewportToWorldPoint(randomPointOutsideScreen[i]);
                randomPointOutsideScreen[i].y = 1f;
            }


            return randomPointOutsideScreen;
        }

        private Vector3[] GenerateSpawnPoints(int amountOfIterations = 1) {
            int direction = Random.Range(0, 4);
            Debug.Log("Direction: " + direction, this);

            Vector3[] spawnPoints = new Vector3[amountOfIterations];

            for (int i = 0; i < amountOfIterations; i++) {
                float x = Random.Range(-0.3f, 1.3f);
                float y = Random.Range(-0.3f, 2.3f);

                switch (direction) {
                    case 0:
                        x = -0.3f;
                        break;
                    case 1:
                        y = 2.3f;
                        break;
                    case 2:
                        x = 1.3f;
                        break;
                    case 3:
                        y = -1.3f;
                        break;
                }

                spawnPoints[i] = new Vector3(x, y, 100);
            }

            return spawnPoints;
        }

        private void OnEnemyKilledEvent(EnemyKilledEvent enemyKilledEvent) {
            enemyDictionary.Remove(enemyKilledEvent.Collider);
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
        }

        public static EnemyManager GetInstance() {
            return instance;
        }
    }
}