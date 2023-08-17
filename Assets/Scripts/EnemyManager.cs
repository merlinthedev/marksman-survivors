using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour {
    private float m_SpawnTimer = 1.4f;

    private bool m_ShouldSpawn = false;
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
            yield return new WaitForSeconds(m_SpawnTimer);
            // Debug.Log("Spawning enemy");
            Enemy enemy = Instantiate(m_EnemyPrefab, CalculateValidSpawnPosition(), Quaternion.identity);
            enemy.SetTarget(m_Player.transform);
            m_EnemyDictionary.Add(enemy.GetComponent<Collider>(), enemy);
        }
    }

    private Vector3 FindPositionRecursively() {
        Vector3 randomPointOnPlane = new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f));


        // If it isn't, check if the random point is in the triangle

        // TODO: GET TWO TRIANGLE POINTS TAKEN FROM THE PLAYERS POSITION ETC ETC 

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

        // draw the randomPointOnPlane
        Debug.DrawLine(playerPos, randomPointOnPlane, Color.red, 0.5f);

        bool isInTriangle = Utilities.IsInsideTriangle(
            new Vector2(playerPos.x, playerPos.z)
            , new Vector2(rightPoint.x, rightPoint.z)
            , new Vector2(leftPoint.x, leftPoint.z)
            , new Vector2(randomPointOnPlane.x, randomPointOnPlane.z));

        // Debug.Log("Point in triangle?: " + isInTriangle);

        if (isInTriangle) {
            return FindPositionRecursively();
        }


        // If the random point is in the triangle, return it
        // If it isn't, try again

        return randomPointOnPlane;
    }

    private Vector3 CalculateValidSpawnPosition() {
        return FindPositionRecursively();
    }

    private void OnEnemyKilledEvent(EnemyKilledEvent enemyKilledEvent) {
        m_EnemyDictionary.Remove(enemyKilledEvent.m_Collider);
    }

    public Enemy GetClosestEnemy(Vector3 position, Enemy enemyToIgnore = null) {
        float closestDistance = Mathf.Infinity;

        Enemy closestEnemy = null;
        foreach (var enemy in m_EnemyDictionary) {
            if (enemy.Value == enemyToIgnore) continue;
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