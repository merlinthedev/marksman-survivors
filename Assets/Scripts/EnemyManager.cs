using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Enemy enemy = Instantiate(m_EnemyPrefab, transform.position + Vector3.up, Quaternion.identity);
            enemy.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
        }
    }


}