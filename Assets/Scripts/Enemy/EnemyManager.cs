﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Champions.Abilities;
using Core;
using Core.Singleton;
using EventBus;
using UnityEngine;
using Util;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Enemy {
    public class EnemyManager : Singleton<EnemyManager> {
        [SerializeField] private Player player; // THIS IS BAD LETS NOT DO THIS
        [SerializeField] private Enemy enemyPrefab;

        // exposed to the editor because there were some unexplainable things happening :D
        [SerializeField] private bool shouldSpawn = false;
        [SerializeField] private int maxAmountOfEnemies = 1;

        [SerializeField] private float secondsBeforeDifficultyIncrease = 60f;

        [SerializeField] [Tooltip("Time it takes between enemy spawn")]
        private float spawnTimer = 1.4f;

        private float lastSpawnTime = 0f;
        private float internalSpawnTimer;
        private float lastDifficultyIncreaseTime = 0f;

        [SerializeField] private float internalMinimumSpawnNumber = 1f;
        [SerializeField] private float internalMaximumSpawnNumber = 2f;

        private float internalSpawnRatio = 1f;
        [SerializeField] private float internalGrowthRate = 1.21f;

        private int amountOfEnemies = 0;
        private Dictionary<Collider, Enemy> enemyDictionary = new();

        private void OnEnable() {
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);

            EventBus<GamePausedEvent>.Subscribe(OnGamePaused);
            EventBus<GameResumedEvent>.Subscribe(OnGameResumed);
        }

        private void OnDisable() {
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);

            EventBus<GamePausedEvent>.Unsubscribe(OnGamePaused);
            EventBus<GameResumedEvent>.Unsubscribe(OnGameResumed);
        }

        private void Start() {
            internalSpawnTimer = Random.Range(spawnTimer - 0.25f, spawnTimer + 0.25f);

            EnemyTimerHandle();
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
            if (!shouldSpawn || Time.timeScale > 1) return;

            // if the time since the last spawn is less than the spawn timer, we don't want to spawn
            if (Time.time - lastSpawnTime < spawnTimer) return;

            if (amountOfEnemies >= maxAmountOfEnemies) {
                shouldSpawn = false;
                return;
            }

            float randomAmountOfEnemies = Random.Range(internalMinimumSpawnNumber * internalSpawnRatio,
                internalMaximumSpawnNumber * internalSpawnRatio);

            // Logger.Log("spawning " + randomAmountOfEnemies + " enemies", Logger.Color.RED, this);
            // Logger.Log("min and max spawn number: " + internalMinimumSpawnNumber * internalSpawnRatio + " " +
            //            internalMaximumSpawnNumber * internalSpawnRatio, Logger.Color.YELLOW, this);

            Vector3[] location = FindPositionsIteratively(Mathf.FloorToInt(randomAmountOfEnemies));

            InstantiateEnemies(location);

            // randomize the spawn timer a bit to make spawns feel more natural
            internalSpawnTimer = Random.Range(spawnTimer - 0.5f, spawnTimer + 0.5f);

            lastSpawnTime = Time.time;
        }

        #region Obsolete

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

        #endregion

        public Vector3[] FindPositionsIteratively(int amountOfIterations = 1) {
            // get a random point on the screen
            Vector3[] randomPointOutsideScreen = GenerateSpawnPoints(amountOfIterations);
            // Debug.Log("Random point on screen: " + randomPointOutsideScreen);

            // convert the from screen space to world space
            for (int i = 0; i < randomPointOutsideScreen.Length; i++) {
                randomPointOutsideScreen[i] = Camera.main.ViewportToWorldPoint(randomPointOutsideScreen[i]);
                randomPointOutsideScreen[i].y = 1f;
            }

            return randomPointOutsideScreen;
        }

        /// <summary>
        /// Generates an array of spawnpoints based on the movement data of the player.
        /// </summary>
        /// <param name="amountOfIterations">
        /// The amount spawnpoints that should be returned.
        /// </param>
        /// <returns>
        /// An array of spawnpoints.
        /// </returns>
        private Vector3[] GenerateSpawnPoints(int amountOfIterations = 1) {
            Vector3[] spawnPoints = new Vector3[amountOfIterations];
            Vector4 movementData = player.GetCurrentlySelectedChampion().GetMovementData();

            for (int i = 0; i < amountOfIterations; i++) {
                float x, y;

                if (i < movementData.x * amountOfIterations) {
                    x = Random.Range(-0.3f, 1.3f);
                    y = 2.3f;
                } else if (i < (movementData.x + movementData.y) * amountOfIterations) {
                    x = 1.3f;
                    y = Random.Range(-0.3f, 2.3f);
                } else if (i < (movementData.x + movementData.y + movementData.z) * amountOfIterations) {
                    x = Random.Range(-0.3f, 1.3f);
                    y = -1.3f;
                } else {
                    x = -0.3f;
                    y = Random.Range(-0.3f, 2.3f);
                }

                spawnPoints[i] = new Vector3(x, y, 100);
            }

            return spawnPoints;
        }

        public Enemy[] InstantiateEnemies(Vector3[] location) {
            Enemy[] enemies = new Enemy[location.Length];
            for (var i = 0; i < location.Length; i++) {
                Vector3 randomSpread = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                Enemy enemy = Instantiate(enemyPrefab, location[i] + randomSpread, Quaternion.Euler(0, 45, 0));
                enemy.SetTarget(player.transform);
                // enemy.SetCanMove(false); // For when we want to test stuff on enemies that should not move
                enemyDictionary.Add(enemy.GetComponent<Collider>(), enemy);
                EventBus<EnemySpawnedEvent>.Raise(new EnemySpawnedEvent(enemy));
                enemies[i] = enemy;
            }
            return enemies;
        }

        public void WipeEnemies() {
            //loop over enemies dictionary and destroy all enemies
            foreach (var kvp in enemyDictionary) {
                Destroy(kvp.Value.gameObject);
            }
            enemyDictionary.Clear();
        }

        private void OnEnemyKilledEvent(EnemyKilledEvent enemyKilledEvent) {
            enemyDictionary.Remove(enemyKilledEvent.collider);
        }

        private void OnGamePaused(GamePausedEvent e) {
            shouldSpawn = false;
            foreach (var kvp in enemyDictionary) {
                kvp.Value.OnPause();
            }
        }

        private void OnGameResumed(GameResumedEvent e) {
            shouldSpawn = true;
            foreach (var kvp in enemyDictionary) {
                kvp.Value?.OnResume();
            }
        }

        /// <summary>
        /// Get the closest enemy to a position.
        /// </summary>
        /// <param name="position">
        /// The position to get the closest enemy to.
        /// </param>
        /// <param name="enemiesToIgnore">
        /// A list of enemies to ignore when getting the closest enemy.
        /// </param>
        /// <returns>
        /// The closest enemy to the position.
        /// </returns>
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

        /// <summary>
        /// Get enemies in a radius around a point.
        /// </summary>
        /// <param name="point">
        /// The point to get enemies around.
        /// </param>
        /// <param name="radius">
        /// The radius to get enemies in.
        /// </param>
        /// <returns>
        /// A list of enemies in the radius around the point.
        /// </returns>
        public List<Enemy> GetEnemiesInArea(Vector3 point, float radius = 10f) {
            return (from enemy in enemyDictionary
                where Vector3.Distance(point, enemy.Value.transform.position) < radius
                select enemy.Value).ToList();
        }

        /// <summary>
        /// Get enemy from collider
        /// </summary>
        /// <param name="other">Collider</param>
        /// <returns>The enemy value from the collider key.</returns>
        /// <exception cref="Exception">Throws an exception if the collider is not present in the dictionary.</exception>
        public Enemy GetEnemy(Collider other) {
            Enemy enemy = enemyDictionary[other];
            if (enemy == null) {
                Logger.Log("Enemy not found in dictionary", Logger.Color.RED, this);
                throw new Exception("Enemy not found in dictionary");
            }

            return enemy;
        }

        /// <summary>
        /// Create the enemy timer.
        /// </summary>
        private void EnemyTimerHandle() {
            var timer = new EnemyTimer(secondsBeforeDifficultyIncrease, secondsBeforeDifficultyIncrease);

            timer.OnCooldownCompleted += IncreaseDifficulty;
        }

        /// <summary>
        /// Increase the spawn ratio by the internal growth rate.
        /// </summary>
        private void IncreaseDifficulty() {
            internalSpawnRatio *= internalGrowthRate;
            // Logger.Log("Increased difficulty, new spawn ratio: " + internalSpawnRatio, Logger.Color.BLUE, this);
        }


        /// <summary>
        /// Set if this object should spawn enemies or not.
        /// </summary>
        /// <param name="shouldSpawn">bool to decide whether to spawn enemies.</param>
        public void SetShouldSpawn(bool shouldSpawn) {
            this.shouldSpawn = shouldSpawn;
        }

        public bool GetShouldSpawn() {
            return shouldSpawn;
        }
    }

    internal class EnemyTimer : ICooldown {
        private readonly float cooldown;
        private float timeLeft;

        public EnemyTimer(float cooldown, float timeLeft) {
            this.cooldown = cooldown;
            this.timeLeft = timeLeft;

            Subscribe(this);
        }

        ~EnemyTimer() {
            Unsubscribe(this);

            OnCooldownCompleted = null;
        }

        public bool ShouldTick => timeLeft > 0;

        public void Tick(float deltaTime) {
            // Debug.LogWarning("Ticking enemy timer: " + timeLeft + ", " + deltaTime);
            timeLeft -= deltaTime;

            // Debug.LogWarning("time left: " + timeLeft);

            if (timeLeft < 0) {
                // Logger.Log("Time left before difficulty increase: " + timeLeft, Logger.Color.BLUE,
                //     GameManager.GetInstance());
                OnCooldownCompleted?.Invoke();
            }
        }

        public void Subscribe(ICooldown cooldown) {
            EventBus<SubscribeICooldownEvent>.Raise(new SubscribeICooldownEvent(cooldown));
        }

        public void Unsubscribe(ICooldown cooldown) {
            EventBus<UnsubscribeICooldownEvent>.Raise(new UnsubscribeICooldownEvent(cooldown));
        }

        public event Action OnCooldownCompleted;

        public float GetCooldown() {
            return cooldown;
        }
    }
}