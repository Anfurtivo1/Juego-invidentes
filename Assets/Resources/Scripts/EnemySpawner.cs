using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;

    // Límites del área
    public Vector3 minBounds = new Vector3(-22.8f, 1, -23f);
    public Vector3 maxBounds = new Vector3(22.8f, 1, 23f);

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 5, 20);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemy()
    {
        Instantiate(enemy, GetRandomNavMeshPosition(), Quaternion.identity);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomPoint = new Vector3(Random.Range(minBounds.x, maxBounds.x), 0, maxBounds.z);
        return randomPoint;
    }
}
