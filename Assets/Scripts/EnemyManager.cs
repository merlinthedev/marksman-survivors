using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour {
    private float m_SpawnTimer = 1.4f;

    private bool m_ShouldSpawn = true;
    [SerializeField] private Enemy m_EnemyPrefab;

    private void Start() {
        StartCoroutine(SpawnEnemy());
    }

    public void StopSpawning() {
        m_ShouldSpawn = false;
    }

    public void StartSpawning() {
        m_ShouldSpawn = true;
    }

    private IEnumerator SpawnEnemy() {
        while (m_ShouldSpawn) {
            yield return new WaitForSeconds(m_SpawnTimer);
            Enemy enemy = Instantiate(m_EnemyPrefab, CalculateValidSpawnPosition(), Quaternion.identity);
            enemy.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
        }
    }

    private Vector3 FindPositionRecursively() {
        Vector2 randomPointOnPlane = new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));

        // Check if the random point is in the viewport
        if (Camera.main.WorldToViewportPoint(randomPointOnPlane).x > 0 &&
            Camera.main.WorldToViewportPoint(randomPointOnPlane).x < 1 &&
            Camera.main.WorldToViewportPoint(randomPointOnPlane).y > 0 &&
            Camera.main.WorldToViewportPoint(randomPointOnPlane).y < 1) {
            // If it is, try again
            return FindPositionRecursively();
        }

        // If it isn't, check if the random point is in the triangle
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Vector3 playerPos = player.gameObject.transform.position;
        Vector3 playerDirection = player.GetCurrentMovementDirection();

        // TODO: GET TWO TRIANGLE POINTS TAKEN FROM THE PLAYERS POSITION ETC ETC 
        
        return Vector3.zero;
    }

    private Vector3 CalculateValidSpawnPosition() {
        return FindPositionRecursively();
    }

}