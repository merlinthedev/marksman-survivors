using System.Collections;
using UnityEngine;
using Util;

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
        Vector3 randomPointOnPlane = new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f));


        // If it isn't, check if the random point is in the triangle
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // TODO: GET TWO TRIANGLE POINTS TAKEN FROM THE PLAYERS POSITION ETC ETC 

        // Take the player position, shoot two rays from this position, one 45 deg to the right of the players direction, one 45 deg to the left
        Vector3 playerPos = player.gameObject.transform.position;
        Vector3 playerDirection = player.GetCurrentlySelectedChampion().GetCurrentMovementDirection();


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
}